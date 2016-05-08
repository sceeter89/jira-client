using JiraAssistant.Model.Jira;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using System.Threading.Tasks;
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
using JiraAssistant.Services.Jira;

namespace JiraAssistant.Pages
{
   public partial class AgileBoardPage : BaseNavigationPage
   {
      private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

      private bool _epicsDownloaded;
      private bool _issuesDownloaded;
      private bool _sprintsDownloaded;
      private bool _isBusy;
      private readonly INavigator _navigator;
      private readonly JiraSessionViewModel _jiraSession;
      private readonly IssuesStatisticsCalculator _statisticsCalculator;
      private IssuesCollectionStatistics _statistics;

      private readonly Dictionary<int, INavigationPage> _sprintsDetailsCache = new Dictionary<int, INavigationPage>();
      private readonly IContainer _iocContainer;
      private readonly IJiraApi _jiraApi;
      private bool _forceReload;
      private bool _downloadCompleted;
      private AgileBoardIssues _boardContent;

      public AgileBoardPage(RawAgileBoard board, IContainer iocContainer)
      {
         InitializeComponent();

         Board = board;

         _iocContainer = iocContainer;
         _jiraApi = iocContainer.Resolve<IJiraApi>();
         _navigator = iocContainer.Resolve<INavigator>();
         _jiraSession = iocContainer.Resolve<JiraSessionViewModel>();
         _statisticsCalculator = iocContainer.Resolve<IssuesStatisticsCalculator>();

         StatusBarControl = new AgileBoardPageStatusBar { DataContext = this };

         SprintDetailsCommand = new RelayCommand(OpenSprintDetails, () => Board.Type == "scrum");
         OpenPivotAnalysisCommand = new RelayCommand(() => _navigator.NavigateTo(new PivotAnalysisPage(BoardContent.Issues)));
         OpenEpicsOverviewCommand = new RelayCommand(() => _navigator.NavigateTo(new EpicsOverviewPage(BoardContent.Issues, BoardContent.Epics, _iocContainer)));
         BrowseIssuesCommand = new RelayCommand(() => _navigator.NavigateTo(new BrowseIssuesPage(BoardContent.Issues, _iocContainer)));
         OpenGraveyardCommand = new RelayCommand(() => _navigator.NavigateTo(new BoardGraveyard(BoardContent.Issues, _iocContainer)));
         OpenSprintsVelocityCommand = new RelayCommand(() => _navigator.NavigateTo(new SprintsVelocity(BoardContent, BoardContent.Sprints, _iocContainer)), () => Board.Type == "scrum");

         RefreshDataCommand = new RelayCommand(() =>
         {
            _forceReload = true;
            DownloadElements();
         }, () => IsBusy == false);
         FetchChangesCommand = new RelayCommand(() =>
         {
            DownloadElements();
         }, () => IsBusy == false);

         Buttons.Add(new ToolbarButton
         {
            Tooltip = "Reload local cache",
            Command = RefreshDataCommand,
            Icon = new BitmapImage(new Uri(@"pack://application:,,,/;component/Assets/Icons/RefreshIcon.png"))
         });
         Buttons.Add(new ToolbarButton
         {
            Tooltip = "Fetch changes since last visit",
            Command = FetchChangesCommand,
            Icon = new BitmapImage(new Uri(@"pack://application:,,,/;component/Assets/Icons/DownloadIcon.png"))
         });

         DataContext = this;
         DownloadElements();
      }

      private void OpenSprintDetails()
      {
         _navigator.NavigateTo(new PickUpSprintPage(BoardContent.Sprints,
            sprint =>
            {
               if (_sprintsDetailsCache.ContainsKey(sprint.Id) == false)
                  _sprintsDetailsCache[sprint.Id] = new SprintDetailsPage(sprint, BoardContent.IssuesInSprint(sprint.Id), _iocContainer);

               return _sprintsDetailsCache[sprint.Id];
            }, _navigator));
      }

      public async void DownloadElements()
      {
         IsBusy = true;
         ClearData();

         try
         {
            BoardContent = await _jiraApi.Agile.GetBoardContent(Board.Id);

            Statistics = await _statisticsCalculator.Calculate(BoardContent.Issues);
            DownloadCompleted = true;
         }
         catch (CacheCorruptedException)
         {
            if (_forceReload)
            {
               MessageBox.Show("Failed to download board data.", "JiraAssistant");
               return;
            }
            MessageBox.Show("There were problems to save board data. We'll try to re-download it.", "Jira Assistant");

            _forceReload = true;
            DownloadElements();
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
         BoardContent = null;
         DownloadCompleted = false;
      }

      public bool DownloadCompleted
      {
         get { return _downloadCompleted; }
         set
         {
            _downloadCompleted = value;
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

      public RawAgileBoard Board { get; private set; }
      public override string Title { get { return string.Format("Board: {0}", Board.Name); } }

      public AgileBoardIssues BoardContent
      {
         get { return _boardContent; }
         set
         {
            _boardContent = value;
            RaisePropertyChanged();
         }
      }

      public RelayCommand SprintDetailsCommand { get; private set; }
      public RelayCommand OpenPivotAnalysisCommand { get; private set; }
      public RelayCommand OpenEpicsOverviewCommand { get; private set; }
      public RelayCommand OpenGraveyardCommand { get; private set; }
      public RelayCommand BrowseIssuesCommand { get; private set; }
      public RelayCommand OpenSprintsVelocityCommand { get; private set; }
      public RelayCommand RefreshDataCommand { get; private set; }
      public RelayCommand FetchChangesCommand { get; private set; }

      public bool IsBusy
      {
         get { return _isBusy; }
         private set
         {
            _isBusy = value;
            RaisePropertyChanged();
            RefreshDataCommand.RaiseCanExecuteChanged();
            FetchChangesCommand.RaiseCanExecuteChanged();
         }
      }
   }
}
