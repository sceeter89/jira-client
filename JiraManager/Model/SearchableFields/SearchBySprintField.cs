using System;
using JiraManager.Api;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.ObjectModel;
using JiraManager.Messages.Actions.Authentication;
using GalaSoft.MvvmLight.Threading;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using System.Windows.Controls;

namespace JiraManager.Model.SearchableFields
{
   public class SearchBySprintField : ViewModelBase, ISearchableField
   {
      private readonly IJiraOperations _operations;
      private RawAgileBoard _selectedBoard;
      private RawAgileSprint _selectedSprint;

      public SearchBySprintField(IMessenger messenger, IJiraOperations operations)
      {
         _operations = operations;

         BoardsList = new ObservableCollection<RawAgileBoard>();
         SprintsList = new ObservableCollection<RawAgileSprint>();

         messenger.Register<LoggedInMessage>(this, async m =>
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

      public ObservableCollection<RawAgileBoard> BoardsList { get; private set; }

      public bool IsFilled
      {
         get
         {
            return SelectedSprint != null;
         }
      }

      public ObservableCollection<RawAgileSprint> SprintsList { get; private set; }

      public RawAgileBoard SelectedBoard
      {
         get { return _selectedBoard; }
         set
         {
            if (_selectedBoard == value)
               return;

            SprintsList.Clear();
            SelectedSprint = null;

            _selectedBoard = value;

            if (value == null)
               return;

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
         }
      }

      public UserControl EditControl
      {
         get
         {
            throw new NotImplementedException();
         }
      }

      public void Clear()
      {
         SelectedSprint = null;
         SelectedBoard = null;
      }

      public string GetSearchQuery()
      {
         return string.Format("Sprint = {0}", SelectedSprint.Id);
      }
   }
}
