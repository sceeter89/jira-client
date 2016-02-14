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

namespace JiraAssistant.Pages
{
   public partial class AgileBoardPage : BaseNavigationPage
   {
      private static Logger _logger = LogManager.GetCurrentClassLogger();

      private bool _epicsDownloaded;
      private bool _issuesDownloaded;
      private bool _sprintsDownloaded;
      private bool _sprintAssignmentDownloaded;
      private readonly JiraAgileService _jiraAgile;
      private readonly IssuesFinder _issuesFinder;
      private bool _isBusy;
      private readonly INavigator _navigator;
      private readonly JiraSessionViewModel _jiraSession;
      private readonly IssuesStatisticsCalculator _statisticsCalculator;
      private IssuesCollectionStatistics _statistics;

      private readonly Dictionary<int, INavigationPage> _sprintsDetailsCache = new Dictionary<int, INavigationPage>();

      public AgileBoardPage(RawAgileBoard board,
         JiraAgileService jiraService,
         IssuesFinder issuesFinder,
         JiraSessionViewModel jiraSession,
         INavigator navigator,
         IssuesStatisticsCalculator statisticsCalculator)
      {
         InitializeComponent();

         Board = board;
         _jiraAgile = jiraService;
         _issuesFinder = issuesFinder;
         _navigator = navigator;
         _jiraSession = jiraSession;
         _statisticsCalculator = statisticsCalculator;

         Epics = new ObservableCollection<RawAgileEpic>();
         Sprints = new ObservableCollection<RawAgileSprint>();
         Issues = new ObservableCollection<JiraIssue>();
         IssuesInSprint = new Dictionary<int, IEnumerable<string>>();

         StatusBarControl = new AgileBoardPageStatusBar { DataContext = this };

         SprintDetailsCommand = new RelayCommand(OpenSprintDetails);
         OpenPivotAnalysisCommand = new RelayCommand(OpenPivotAnalysis);
         OpenEpicsOverviewCommand = new RelayCommand(OpenEpicsOverview);
         BrowseIssuesCommand = new RelayCommand(BrowseIssues);

         RefreshDataCommand = new RelayCommand(() => DownloadElements(), () => IsBusy == false);

         Buttons.Add(new ToolbarButton
         {
            Tooltip = "Refresh data",
            Command = RefreshDataCommand,
            Icon = new BitmapImage(new Uri(@"pack://application:,,,/;component/Assets/Icons/RefreshIcon.png"))
         });

         DataContext = this;
         DownloadElements();
      }

      private void BrowseIssues()
      {
         _navigator.NavigateTo(new BrowseIssuesPage(Issues, _navigator));
      }

      private void OpenEpicsOverview()
      {
         _navigator.NavigateTo(new EpicsOverviewPage(Issues, Epics));
      }

      private void OpenPivotAnalysis()
      {
         _navigator.NavigateTo(new PivotAnalysisPage(Issues));
      }

      private void OpenSprintDetails()
      {
         _navigator.NavigateTo(new PickUpSprintPage(Sprints,
            sprint =>
            {
               if (_sprintsDetailsCache.ContainsKey(sprint.Id) == false)
                  _sprintsDetailsCache[sprint.Id] = new SprintDetailsPage(sprint, Issues.Where(i => IssuesInSprint[sprint.Id].Contains(i.Key)).ToList(), _navigator, _statisticsCalculator);

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
         Statistics = await _statisticsCalculator.Calculate(Issues);
      }

      private void ClearData()
      {
         Issues.Clear();
         IssuesDownloaded = false;

         Sprints.Clear();
         IssuesInSprint.Clear();
         SprintsDownloaded = false;
         SprintAssignmentDownloaded = false;

         Epics.Clear();
         EpicsDownloaded = false;
      }

      private async Task DownloadIssues()
      {
         var boardConfig = await _jiraAgile.GetBoardConfiguration(Board.Id);
         var issues = await _issuesFinder.Search(boardConfig.Filter);

         foreach (var issue in issues)
         {
            Issues.Add(issue);
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

         SprintsDownloaded = true;

         foreach (var sprint in sprints.OrderByDescending(s => s.StartDate))
         {
            Sprints.Add(sprint);
         }

         foreach (var sprint in sprints)
         {
            var issuesInSprint = await _jiraAgile.GetIssuesInSprint(Board.Id, sprint.Id);
            IssuesInSprint[sprint.Id] = issuesInSprint;
         }

         SprintAssignmentDownloaded = true;
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

      public bool SprintAssignmentDownloaded
      {
         get { return _sprintAssignmentDownloaded; }
         set
         {
            _sprintAssignmentDownloaded = value;
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
      public IDictionary<int, IEnumerable<string>> IssuesInSprint { get; private set; }

      public ICommand SprintDetailsCommand { get; private set; }
      public ICommand OpenPivotAnalysisCommand { get; private set; }
      public ICommand OpenEpicsOverviewCommand { get; private set; }

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
      public RelayCommand BrowseIssuesCommand { get; private set; }
   }
}
