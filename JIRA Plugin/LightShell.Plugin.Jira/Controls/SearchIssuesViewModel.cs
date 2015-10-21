using LightShell.Api;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using LightShell.Messaging.Api;
using System.Windows;
using GalaSoft.MvvmLight;
using LightShell.Plugin.Jira.Api.Messages.IO.Jira;
using LightShell.Plugin.Jira.Api.Messages.Actions;
using LightShell.Plugin.Jira.Api.Model;
using GalaSoft.MvvmLight.Command;
using LightShell.Plugin.Jira.Controls.SearchFields;
using LightShell.Plugin.Jira.Api.Messages.Actions.Authentication;

namespace LightShell.Plugin.Jira.Controls
{
   internal class SearchIssuesViewModel : ViewModelBase,
      IHandleMessage<SearchForIssuesResponse>,
      IHandleMessage<SearchFailedResponse>,
      IHandleMessage<CurrentSearchResultsMessage>
   {
      private readonly IMessageBus _messageBus;
      private bool _isBusy = false;
      private string _searchQuery;

      public SearchIssuesViewModel(IMessageBus messageBus)
      {
         _messageBus = messageBus;
         _messageBus.Register(this);

         FoundIssues = new ObservableCollection<JiraIssue>();

         SearchableFields = new ISearchableField[]
         {
            new SearchBySprintField(messageBus),
            new ComboBoxSearchField<RawIssueType, GetIssueTypesMessage, GetIssueTypesResponse>(_messageBus,
                                                  r=> r.IssueTypes,
                                                  x => x.Name,
                                                  x => x.Name,
                                                  "issuetype",
                                                  "Select issue type"),
            new ComboBoxSearchField<RawProjectInfo, GetProjectsMessage, GetProjectsResponse>(_messageBus,
                                                  r => r.Projects,
                                                  x => x.Name,
                                                  x => x.Name,
                                                  "project",
                                                  "Select project"),
            new ComboBoxSearchField<RawPriority, GetPrioritiesMessage, GetPrioritiesResponse>(_messageBus,
                                                  r => r.Priorities,
                                                  x => x.Name,
                                                  x => x.Name,
                                                  "priority",
                                                  "Select priority"),
            new ComboBoxSearchField<RawResolution, GetResolutionsMessage, GetResolutionsResponse>(_messageBus,
                                                  r => r.Resolutions,
                                                  x => x.Name,
                                                  x => x.Name,
                                                  "resolution",
                                                  "Select resolution"),
            new ComboBoxSearchField<RawStatus, GetStatusesMessage, GetStatusesResponse>(_messageBus,
                                                  r => r.Statuses,
                                                  x => x.Name,
                                                  x => x.Name,
                                                  "status",
                                                  "Select status")
         };

         _messageBus.Send(new IsLoggedInMessage());
      }

      private void DoCustomSearch()
      {
         var searchClauses = SearchableFields.Where(f => f.IsFilled).Select(f => f.GetSearchQuery());
         var searchString = string.Join(" AND ", searchClauses.Select(c => string.Format("( {0} )", c)));

         SearchQuery = searchString;
         DoSearch();
      }

      private void DoSearch()
      {
         SetIsBusy(true);
         _messageBus.LogMessage("Initiating search for issues by JQL query", LogLevel.Info);

         FoundIssues.Clear();
         if (string.IsNullOrWhiteSpace(SearchQuery))
         {
            var dialog = MessageBox.Show("You are about to search for all issues in JIRA instance. Are you sure?", "Jira Client", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (dialog == MessageBoxResult.No)
            {
               SetIsBusy(false);
               return;
            }
            SearchQuery = "";
         }

         _messageBus.Send(new SearchForIssuesMessage(SearchQuery));
      }

      private void SetIsBusy(bool isBusy)
      {
         _isBusy = isBusy;
         SearchCommand.RaiseCanExecuteChanged();
         CustomSearchCommand.RaiseCanExecuteChanged();
      }

      public void Handle(SearchForIssuesResponse message)
      {
         _messageBus.LogMessage("Loading results...");
         foreach (var issue in message.SearchResults)
         {
            FoundIssues.Add(issue);
         }
         _messageBus.LogMessage(string.Format("Search done. Found {0} issues.", FoundIssues.Count));
         SetIsBusy(false);
      }

      public void Handle(SearchFailedResponse message)
      {
         switch (message.Reason)
         {
            case SearchFailedResponse.FailureReason.ExceptionOccured:
               _messageBus.LogMessage("Search was interrupted by an exception.", LogLevel.Critical);
               break;
            case SearchFailedResponse.FailureReason.NoResultsYielded:
               _messageBus.LogMessage("No issues found.", LogLevel.Info);
               break;
         }
         SetIsBusy(false);
      }

      public void Handle(CurrentSearchResultsMessage message)
      {
         _messageBus.Send(new CurrentSearchResultsResponse(FoundIssues));
      }

      private RelayCommand _searchCommand;
      public RelayCommand SearchCommand
      {
         get
         {
            if (_searchCommand == null)
               _searchCommand = new RelayCommand(DoSearch, () => _isBusy == false);

            return _searchCommand;
         }
      }
      private RelayCommand _customSearchCommand;
      public RelayCommand CustomSearchCommand
      {
         get
         {
            if (_customSearchCommand == null)
               _customSearchCommand = new RelayCommand(DoCustomSearch, () => _isBusy == false);

            return _customSearchCommand;
         }
      }
      public ObservableCollection<JiraIssue> FoundIssues { get; private set; }
      public IEnumerable<ISearchableField> SearchableFields { get; private set; }

      public string SearchQuery
      {
         get { return _searchQuery; }
         set
         {
            _searchQuery = value;
            RaisePropertyChanged();
         }
      }
   }
}
