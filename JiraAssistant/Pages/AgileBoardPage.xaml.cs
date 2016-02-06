using JiraAssistant.Model.Jira;
using JiraAssistant.Services.Resources;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace JiraAssistant.Pages
{
   public partial class AgileBoardPage : BaseNavigationPage
   {
      private readonly RawAgileBoard _board;
      private bool _epicsDownloaded;
      private bool _issuesDownloaded;
      private bool _sprintsDownloaded;
      private bool _sprintAssignmentDownloaded;
      private readonly JiraAgileService _jiraService;

      public AgileBoardPage(RawAgileBoard board,
         JiraAgileService jiraService)
      {
         InitializeComponent();
         _board = board;
         _jiraService = jiraService;
         DataContext = this;
         DownloadElements();
      }

      public void DownloadElements()
      {
         var downloadSprintsTask = _jiraService.GetSprints(_board.Id);
         var downloadEpicsTask = _jiraService.GetEpics(_board.Id);
         
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
      public IDictionary<int, IList<string>> IssuesInSprint { get; private set; }
   }
}
