using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using JiraManager.Api;
using System.Collections.ObjectModel;
using JiraManager.Model;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using JiraManager.Helpers;
using JiraManager.Messages.Actions;
using JiraManager.Model.SearchableFields;

namespace JiraManager.ViewModel
{
   public class SearchIssuesViewModel : ViewModelBase
   {
      private readonly IMessenger _messenger;
      private bool _isBusy = false;
      private readonly BackgroundWorker _backgroundWorker = new BackgroundWorker();
      private readonly IJiraOperations _operations;
      private RawIssueToJiraIssue _modelConverter;
      private string _searchQuery;

      public SearchIssuesViewModel(IMessenger messenger, IJiraOperations operations)
      {
         _operations = operations;
         _messenger = messenger;
         SearchCommand = new RelayCommand(DoSearch, () => _isBusy == false);
         CustomSearchCommand = new RelayCommand(DoCustomSearch, () => _isBusy == false);

         FoundIssues = new ObservableCollection<JiraIssue>();
         _backgroundWorker.DoWork += DownloadIssues;
         _backgroundWorker.RunWorkerCompleted += DownloadCompleted;

         SearchableFields = new[]
         {
            (ISearchableField)new SearchBySprintField(messenger, operations),
            (ISearchableField)new SearchByIssueTypeField(messenger, operations)
         };
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
         _messenger.Send(new NewSearchResultsAvailable());
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
