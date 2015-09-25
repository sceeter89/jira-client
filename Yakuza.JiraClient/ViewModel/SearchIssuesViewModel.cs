using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Yakuza.JiraClient.Api;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using Yakuza.JiraClient.Helpers;
using Yakuza.JiraClient.Api.Model;
using Yakuza.JiraClient.IssueFields.Search;
using Yakuza.JiraClient.Api.Messages.Actions.Authentication;
using Yakuza.JiraClient.Messaging.Api;
using Yakuza.JiraClient.Api.Messages.IO.Jira;

namespace Yakuza.JiraClient.ViewModel
{
   public class SearchIssuesViewModel : ViewModelBase
   {
      private readonly IMessageBus _messenger;
      private bool _isBusy = false;
      private readonly BackgroundWorker _backgroundWorker = new BackgroundWorker();
      private readonly IJiraOperations _operations;
      private RawIssueToJiraIssue _modelConverter;
      private string _searchQuery;

      public SearchIssuesViewModel(IMessageBus messenger, IJiraOperations operations)
      {
         _operations = operations;
         _messenger = messenger;
         SearchCommand = new RelayCommand(DoSearch, () => _isBusy == false);
         CustomSearchCommand = new RelayCommand(DoCustomSearch, () => _isBusy == false);

         FoundIssues = new ObservableCollection<JiraIssue>();
         _backgroundWorker.DoWork += DownloadIssues;
         _backgroundWorker.RunWorkerCompleted += DownloadCompleted;

         SearchableFields = new ISearchableField[]
         {
            new SearchBySprintField(messenger, operations),
            new ComboBoxSearchField<RawIssueType>(_messenger,
                                                  ()=> _operations.GetIssueTypes(),
                                                  x => x.Name,
                                                  x => x.Name,
                                                  "issuetype",
                                                  "Select issue type"),
            new ComboBoxSearchField<RawProjectInfo>(_messenger,
                                                  ()=> _operations.GetProjectsList(),
                                                  x => x.Name,
                                                  x => x.Name,
                                                  "project",
                                                  "Select project"),
            new ComboBoxSearchField<RawPriority>(_messenger,
                                                  ()=> _operations.GetPrioritiesList(),
                                                  x => x.Name,
                                                  x => x.Name,
                                                  "priority",
                                                  "Select priority"),
            new ComboBoxSearchField<RawResolution>(_messenger,
                                                  ()=> _operations.GetResolutionsList(),
                                                  x => x.Name,
                                                  x => x.Name,
                                                  "resolution",
                                                  "Select resolution")
         };

         _messenger.Send(new IsLoggedInMessage());
      }

      private void DoCustomSearch()
      {
         var searchClauses = SearchableFields.Where(f => f.IsFilled).Select(f => f.GetSearchQuery());
         var searchString = string.Join(" AND ", searchClauses.Select(c => string.Format("( {0} )", c)));

         SearchQuery = searchString;
         SearchCommand.Execute(null);
      }

      private void DownloadCompleted(object sender, RunWorkerCompletedEventArgs e)
      {
         if (e.Error != null)
         {
            _messenger.LogMessage(e.Error.Message);
            _messenger.LogMessage("Exception occured during download!", LogLevel.Critical);
            SetIsBusy(false);
            return;
         }
         if (e.Cancelled)
         {
            _messenger.LogMessage("Searching was cancelled by user.", LogLevel.Info);
            SetIsBusy(false);
            return;
         }
         if (e.Result is string)
         {
            _messenger.LogMessage("Search failed with following message: " + Environment.NewLine + (string)e.Result);
            SetIsBusy(false);
            return;
         }
         if (e.Result is IEnumerable<RawIssue> == false)
         {
            _messenger.LogMessage("Search didn't produce any resuly.");
            SetIsBusy(false);
            return;
         }

         var results = e.Result as IEnumerable<RawIssue>;
         _messenger.LogMessage("Loading results...");
         foreach (var issue in results.Select(_modelConverter.Convert))
         {
            FoundIssues.Add(issue);
         }
         _messenger.LogMessage(string.Format("Search done. Found {0} issues.", FoundIssues.Count));
         _messenger.Send(new SearchForIssuesResponse(FoundIssues));
         SetIsBusy(false);
      }

      private void DownloadIssues(object sender, DoWorkEventArgs e)
      {
         var query = e.Argument as string;
         if (query == null)
         {
            e.Result = "Query is not valid string";
            return;
         }

         if (_modelConverter == null)
         {
            var definitions = _operations.GetFieldDefinitions().Result;
            _modelConverter = new RawIssueToJiraIssue(definitions);
         }

         var results = _operations.GetIssues(query).Result;
         e.Result = results;
      }

      private void DoSearch()
      {
         SetIsBusy(true);
         _messenger.LogMessage(SearchQuery);
         _messenger.LogMessage("Initiating search for issues by JQL query", LogLevel.Info);

         FoundIssues.Clear();
         _backgroundWorker.RunWorkerAsync(SearchQuery);
      }

      private void SetIsBusy(bool isBusy)
      {
         _isBusy = isBusy;
         SearchCommand.RaiseCanExecuteChanged();
      }

      public RelayCommand SearchCommand { get; private set; }

      public RelayCommand CustomSearchCommand { get; private set; }

      public ObservableCollection<JiraIssue> FoundIssues { get; private set; }

      public string SearchQuery
      {
         get { return _searchQuery; }
         set
         {
            _searchQuery = value;
            RaisePropertyChanged();
         }
      }

      public IEnumerable<ISearchableField> SearchableFields { get; private set; }
   }
}
