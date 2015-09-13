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
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Threading;
using JiraManager.Messages.Actions.Authentication;
using JiraManager.Messages.Actions;

namespace JiraManager.ViewModel
{
   public class SearchIssuesViewModel : ViewModelBase
   {
      private readonly IMessenger _messenger;
      private bool _isBusy = false;
      private readonly BackgroundWorker _backgroundWorker = new BackgroundWorker();
      private readonly IJiraOperations _operations;
      private RawIssueToJiraIssue _modelConverter;
      private RawAgileBoard _selectedBoard;
      private RawAgileSprint _selectedSprint;
      private string _searchQuery;

      public SearchIssuesViewModel(IMessenger messenger, IJiraOperations operations)
      {
         _operations = operations;
         _messenger = messenger;
         SearchCommand = new RelayCommand(DoSearch, () => _isBusy == false);
         SearchBySprintCommand = new RelayCommand(() =>
         {
            SearchQuery = string.Format("Sprint = {0}", SelectedSprint.Id);
            SearchCommand.Execute(null);
         }, () => SelectedSprint != null);
         FoundIssues = new ObservableCollection<JiraIssue>();
         _backgroundWorker.DoWork += DownloadIssues;
         _backgroundWorker.RunWorkerCompleted += DownloadCompleted;

         BoardsList = new ObservableCollection<RawAgileBoard>();
         SprintsList = new ObservableCollection<RawAgileSprint>();

         _messenger.Register<LoggedInMessage>(this, async m =>
         {
            var boards = await _operations.GetAgileBoards();
            if (boards == null)
               return;
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
               BoardsList.Clear();
               foreach (var board in boards.Where(x => x.Type == "scrum").OrderBy(x => x.Name))
                  BoardsList.Add(board);
            });
         });
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

      private async void DoSearch()
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

      public RelayCommand SearchBySprintCommand { get; private set; }

      public ObservableCollection<JiraIssue> FoundIssues { get; private set; }

      public ObservableCollection<RawAgileBoard> BoardsList { get; private set; }
      public ObservableCollection<RawAgileSprint> SprintsList { get; private set; }

      public RawAgileBoard SelectedBoard
      {
         get { return _selectedBoard; }
         set
         {
            SprintsList.Clear();
            SelectedSprint = null;

            _selectedBoard = value;

            Task.Run(async () =>
            {
               var sprints = await _operations.GetSprintsForBoard(value.Id);
               DispatcherHelper.CheckBeginInvokeOnUI(() =>
               {
                  SprintsList.Clear();
                  foreach (var sprint in sprints.OrderBy(x => x.Name))
                     SprintsList.Add(sprint);
               });
            });

            RaisePropertyChanged();
         }
      }

      public RawAgileSprint SelectedSprint
      {
         get { return _selectedSprint; }
         set
         {
            _selectedSprint = value;
            RaisePropertyChanged();
            SearchBySprintCommand.RaiseCanExecuteChanged();
         }
      }

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
