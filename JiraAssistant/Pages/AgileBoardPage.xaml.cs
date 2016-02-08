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

namespace JiraAssistant.Pages
{
   public partial class AgileBoardPage : BaseNavigationPage
   {
      private readonly RawAgileBoard _board;
      private bool _epicsDownloaded;
      private bool _issuesDownloaded;
      private bool _sprintsDownloaded;
      private bool _sprintAssignmentDownloaded;
      private readonly JiraAgileService _jiraAgile;
      private readonly IssuesFinder _issuesFinder;
      private bool _isBusy;
      private readonly INavigator _navigator;

      public AgileBoardPage(RawAgileBoard board,
         JiraAgileService jiraService,
         IssuesFinder issuesFinder,
         INavigator navigator)
      {
         InitializeComponent();

         _board = board;
         _jiraAgile = jiraService;
         _issuesFinder = issuesFinder;
         _navigator = navigator;

         Epics = new ObservableCollection<RawAgileEpic>();
         Sprints = new ObservableCollection<RawAgileSprint>();
         Issues = new ObservableCollection<JiraIssue>();
         IssuesInSprint = new Dictionary<int, IEnumerable<string>>();

         StatusBarControl = new AgileBoardPageStatusBar { DataContext = this };

         SprintDetailsCommand = new RelayCommand(OpenSprintDetails);
         OpenPivotAnalysisCommand = new RelayCommand(OpenPivotAnalysis);
         OpenEpicsOverviewCommand = new RelayCommand(OpenEpicsOverview);
         OpenSprintsOverviewCommand = new RelayCommand(OpenSprintsOverview, () => false);

         DataContext = this;
         DownloadElements();
      }

      private void OpenSprintsOverview()
      {
         throw new NotImplementedException();
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
            sprint => new SprintDetailsPage(sprint, Issues.Where(i => IssuesInSprint[sprint.Id].Contains(i.Key)), _navigator),
            _navigator));
      }

      public async void DownloadElements()
      {
         IsBusy = true;
         Issues.Clear();
         IssuesDownloaded = false;

         Sprints.Clear();
         IssuesInSprint.Clear();
         SprintsDownloaded = false;
         SprintAssignmentDownloaded = false;

         Epics.Clear();
         EpicsDownloaded = false;

         var sprintsTask = DownloadSprints();
         var epicsTask = DownloadEpics();
         var issuesTask = DownloadIssues();

         await Task.Factory.StartNew(() => Task.WaitAll(sprintsTask, epicsTask, issuesTask));
         IsBusy = false;
      }

      private async Task DownloadIssues()
      {
         var boardConfig = await _jiraAgile.GetBoardConfiguration(_board.Id);
         var issues = await _issuesFinder.Search(boardConfig.Filter);

         foreach (var issue in issues)
         {
            Issues.Add(issue);
         }

         IssuesDownloaded = true;
      }

      private async Task DownloadEpics()
      {
         var epics = await _jiraAgile.GetEpics(_board.Id);

         foreach (var epic in epics)
         {
            Epics.Add(epic);
         }

         EpicsDownloaded = true;
      }

      private async Task DownloadSprints()
      {
         var sprints = await _jiraAgile.GetSprints(_board.Id);

         SprintsDownloaded = true;

         foreach (var sprint in sprints.OrderByDescending(s => s.StartDate))
         {
            Sprints.Add(sprint);
         }

         foreach (var sprint in sprints)
         {
            var issuesInSprint = await _jiraAgile.GetIssuesInSprint(_board.Id, sprint.Id);
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

      public ObservableCollection<RawAgileEpic> Epics { get; private set; }
      public ObservableCollection<RawAgileSprint> Sprints { get; private set; }
      public ObservableCollection<JiraIssue> Issues { get; private set; }
      public IDictionary<int, IEnumerable<string>> IssuesInSprint { get; private set; }

      public ICommand SprintDetailsCommand { get; private set; }
      public ICommand OpenPivotAnalysisCommand { get; private set; }
      public ICommand OpenEpicsOverviewCommand { get; private set; }
      public ICommand OpenSprintsOverviewCommand { get; private set; }

      public bool IsBusy
      {
         get { return _isBusy; }
         private set
         {
            _isBusy = value;
            RaisePropertyChanged();
         }
      }
   }
}
