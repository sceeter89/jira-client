using JiraAssistant.Model.Jira;
using JiraAssistant.Services.Resources;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;

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

      public AgileBoardPage(RawAgileBoard board,
         JiraAgileService jiraService,
         IssuesFinder issuesFinder)
      {
         InitializeComponent();

         _board = board;
         _jiraAgile = jiraService;
         _issuesFinder = issuesFinder;

         Epics = new ObservableCollection<RawAgileEpic>();
         Sprints = new ObservableCollection<RawAgileSprint>();
         Issues = new ObservableCollection<JiraIssue>();
         IssuesInSprint = new Dictionary<int, IEnumerable<string>>();

         StatusBarControl = new AgileBoardPageStatusBar { DataContext = this };

         DataContext = this;
         DownloadElements();
      }

      public void DownloadElements()
      {
         Issues.Clear();
         IssuesDownloaded = false;

         Sprints.Clear();
         IssuesInSprint.Clear();
         SprintsDownloaded = false;
         SprintAssignmentDownloaded = false;

         Epics.Clear();
         EpicsDownloaded = false;

         DownloadSprints();
         DownloadEpics();
         DownloadIssues();
      }

      private async void DownloadIssues()
      {
         var boardConfig = await _jiraAgile.GetBoardConfiguration(_board.Id);
         var issues = await _issuesFinder.Search(boardConfig.Filter);

         foreach (var issue in issues)
         {
            Issues.Add(issue);
         }

         IssuesDownloaded = true;
      }

      private async void DownloadEpics()
      {
         var epics = await _jiraAgile.GetEpics(_board.Id);

         foreach (var epic in epics)
         {
            Epics.Add(epic);
         }

         EpicsDownloaded = true;
      }

      private async void DownloadSprints()
      {
         var sprints = await _jiraAgile.GetSprints(_board.Id);

         SprintsDownloaded = true;

         foreach (var sprint in sprints)
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
         get
         {
            return _epicsDownloaded;
         }
         set
         {
            _epicsDownloaded = value;
            RaisePropertyChanged();
         }
      }

      public bool SprintsDownloaded
      {
         get
         {
            return _sprintsDownloaded;
         }
         set
         {
            _sprintsDownloaded = value;
            RaisePropertyChanged();
         }
      }

      public bool IssuesDownloaded
      {
         get
         {
            return _issuesDownloaded;
         }
         set
         {
            _issuesDownloaded = value;
            RaisePropertyChanged();
         }
      }

      public bool SprintAssignmentDownloaded
      {
         get
         {
            return _sprintAssignmentDownloaded;
         }
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
   }
}
