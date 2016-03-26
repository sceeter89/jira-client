using JiraAssistant.Model.Jira;
using JiraAssistant.Services.Resources;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Linq;
using GalaSoft.MvvmLight.Command;
using JiraAssistant.Services;
using JiraAssistant.Model.Ui;
using System.Windows.Media.Imaging;
using System.Windows;
using NLog;
using System.Net;
using JiraAssistant.ViewModel;
using JiraAssistant.Model;
using Autofac;
using JiraAssistant.Model.Exceptions;
using JiraAssistant.Controls;

namespace JiraAssistant.Pages
{
   public partial class AgileBoardPage : BaseNavigationPage
   {
      private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

      private bool _epicsDownloaded;
      private bool _issuesDownloaded;
      private bool _sprintsDownloaded;
      private readonly JiraAgileService _jiraAgile;
      private readonly IssuesFinder _issuesFinder;
      private bool _isBusy;
      private readonly INavigator _navigator;
      private readonly JiraSessionViewModel _jiraSession;
      private readonly IssuesStatisticsCalculator _statisticsCalculator;
      private IssuesCollectionStatistics _statistics;

      private readonly Dictionary<int, INavigationPage> _sprintsDetailsCache = new Dictionary<int, INavigationPage>();
      private readonly ApplicationCache _cache;
      private readonly MetadataRetriever _retriever;
      private readonly IContainer _iocContainer;
      private bool _cacheLoaded;
      private readonly AgileBoardDataCache _boardCache;

      public AgileBoardPage(RawAgileBoard board, IContainer iocContainer)
      {
         InitializeComponent();

         Board = board;

         _iocContainer = iocContainer;
         _jiraAgile = iocContainer.Resolve<JiraAgileService>();
         _issuesFinder = iocContainer.Resolve<IssuesFinder>();
         _navigator = iocContainer.Resolve<INavigator>();
         _jiraSession = iocContainer.Resolve<JiraSessionViewModel>();
         _statisticsCalculator = iocContainer.Resolve<IssuesStatisticsCalculator>();
         _cache = iocContainer.Resolve<ApplicationCache>();
         _boardCache = _cache.GetAgileBoardCache(Board.Id);
         _retriever = iocContainer.Resolve<MetadataRetriever>();

         Epics = new ObservableCollection<RawAgileEpic>();
         Sprints = new ObservableCollection<RawAgileSprint>();
         Issues = new ObservableCollection<JiraIssue>();
         IssuesInSprint = new Dictionary<int, IList<JiraIssue>>();

         StatusBarControl = new AgileBoardPageStatusBar { DataContext = this };

         SprintDetailsCommand = new RelayCommand(OpenSprintDetails, () => Board.Type == "scrum");
         OpenPivotAnalysisCommand = new RelayCommand(() => _navigator.NavigateTo(new PivotAnalysisPage(Issues)));
         OpenEpicsOverviewCommand = new RelayCommand(() => _navigator.NavigateTo(new EpicsOverviewPage(Issues, Epics)));
         BrowseIssuesCommand = new RelayCommand(() => _navigator.NavigateTo(new BrowseIssuesPage(Issues, _iocContainer)));
         OpenGraveyardCommand = new RelayCommand(() => _navigator.NavigateTo(new BoardGraveyard(Issues, _iocContainer)));
         OpenSprintsVelocityCommand = new RelayCommand(() => _navigator.NavigateTo(new SprintsVelocity(IssuesInSprint, Sprints)), () => Board.Type == "scrum");

         RefreshDataCommand = new RelayCommand(() =>
         {
            _boardCache.Invalidate();
            DownloadElements();
         }, () => IsBusy == false);

         Buttons.Add(new ToolbarButton
         {
            Tooltip = "Refresh data",
            Command = RefreshDataCommand,
            Icon = new BitmapImage(new Uri(@"pack://application:,,,/;component/Assets/Icons/RefreshIcon.png"))
         });

         DataContext = this;
         DownloadElements();
      }

      private void OpenSprintDetails()
      {
         _navigator.NavigateTo(new PickUpSprintPage(Sprints,
            sprint =>
            {
               if (_sprintsDetailsCache.ContainsKey(sprint.Id) == false)
                  _sprintsDetailsCache[sprint.Id] = new SprintDetailsPage(sprint, IssuesInSprint[sprint.Id], _iocContainer);

               return _sprintsDetailsCache[sprint.Id];
            }, _navigator));
      }

      public async void DownloadElements()
      {
         IsBusy = true;
         ClearData();

         try
         {
            var sprintsTask = DownloadSprints();
            var epicsTask = DownloadEpics();
            var issuesTask = DownloadIssues();

            await Task.Factory.StartNew(() => Task.WaitAll(sprintsTask, epicsTask, issuesTask));

            Statistics = await _statisticsCalculator.Calculate(Issues);
         }
         catch (CacheCorruptedException)
         {
            _boardCache.Invalidate();
            MessageBox.Show("There were problems to save board data. We'll try to re-download it.", "Jira Assistant");
            await DownloadIssues();
         }
         catch (Exception e)
         {
            if (e is WebException && _jiraSession.IsLoggedIn == false)
            {
               _logger.Trace("Download interrupted due to log out from JIRA.");
               return;
            }

            _logger.Error(e, "Download failed for issues in board: {0} ({1})", Board.Name, Board.Id);
            ClearData();
            MessageBox.Show("Failed to retrieve issues belonging board: " + Board.Name, "Jira Assistant");
         }
         finally
         {
            IsBusy = false;
         }
      }

      private void ClearData()
      {
         Issues.Clear();
         IssuesDownloaded = false;
         CacheLoaded = false;

         Sprints.Clear();
         IssuesInSprint.Clear();
         SprintsDownloaded = false;

         Epics.Clear();
         EpicsDownloaded = false;
      }

      private async Task DownloadIssues()
      {
         var boardConfig = await _jiraAgile.GetBoardConfiguration(Board.Id);
         var filter = await _retriever.GetFilterDefinition(boardConfig.Filter.Id);

         var issues = await _issuesFinder.Search(_boardCache.PrepareSearchStatement(filter.Jql));

         issues = await _boardCache.UpdateCache(issues);

         CacheLoaded = true;

         foreach (var issue in issues)
         {
            Issues.Add(issue);
            foreach (var sprintId in issue.SprintIds)
            {
               if (IssuesInSprint.ContainsKey(sprintId) == false)
                  IssuesInSprint[sprintId] = new List<JiraIssue>();

               IssuesInSprint[sprintId].Add(issue);
            }
         }

         IssuesDownloaded = true;
      }

      private async Task DownloadEpics()
      {
         var epics = await _jiraAgile.GetEpics(Board.Id);

         foreach (var epic in epics)
         {
            Epics.Add(epic);
         }

         EpicsDownloaded = true;
      }

      private async Task DownloadSprints()
      {
         var sprints = await _jiraAgile.GetSprints(Board.Id);

         foreach (var sprint in sprints.OrderByDescending(s => s.StartDate))
         {
            Sprints.Add(sprint);
         }

         SprintsDownloaded = true;
      }

      public bool EpicsDownloaded
      {
         get { return _epicsDownloaded; }
         set
         {
            _epicsDownloaded = value;
            RaisePropertyChanged();
         }
      }

      public bool SprintsDownloaded
      {
         get { return _sprintsDownloaded; }
         set
         {
            _sprintsDownloaded = value;
            RaisePropertyChanged();
         }
      }

      public bool IssuesDownloaded
      {
         get { return _issuesDownloaded; }
         set
         {
            _issuesDownloaded = value;
            RaisePropertyChanged();
         }
      }

      public bool CacheLoaded
      {
         get { return _cacheLoaded; }
         set
         {
            _cacheLoaded = value;
            RaisePropertyChanged();
         }
      }

      public IssuesCollectionStatistics Statistics
      {
         get { return _statistics; }
         set
         {
            _statistics = value;
            RaisePropertyChanged();
         }
      }

      public ObservableCollection<RawAgileEpic> Epics { get; private set; }
      public ObservableCollection<RawAgileSprint> Sprints { get; private set; }
      public ObservableCollection<JiraIssue> Issues { get; private set; }
      public IDictionary<int, IList<JiraIssue>> IssuesInSprint { get; private set; }

      public RelayCommand SprintDetailsCommand { get; private set; }
      public RelayCommand OpenPivotAnalysisCommand { get; private set; }
      public RelayCommand OpenEpicsOverviewCommand { get; private set; }
      public RelayCommand OpenGraveyardCommand { get; private set; }
      public RelayCommand BrowseIssuesCommand { get; private set; }
      public RelayCommand OpenSprintsVelocityCommand { get; private set; }

      public bool IsBusy
      {
         get { return _isBusy; }
         private set
         {
            _isBusy = value;
            RaisePropertyChanged();
            RefreshDataCommand.RaiseCanExecuteChanged();
         }
      }

      public RelayCommand RefreshDataCommand { get; private set; }
      public RawAgileBoard Board { get; private set; }
   }
}
