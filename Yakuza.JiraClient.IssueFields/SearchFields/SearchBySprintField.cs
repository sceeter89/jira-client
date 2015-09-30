using Yakuza.JiraClient.Api;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Threading;
using System.Linq;
using GalaSoft.MvvmLight;
using System.Windows.Controls;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using Yakuza.JiraClient.Api.Model;
using Yakuza.JiraClient.Api.Messages.Actions.Authentication;
using Yakuza.JiraClient.Api.Messages.Status;
using Yakuza.JiraClient.Messaging.Api;
using Yakuza.JiraClient.Api.Messages.IO.Jira;

namespace Yakuza.JiraClient.IssueFields.Search
{
   public class SearchBySprintField : ViewModelBase, ISearchableField,
      IHandleMessage<LoggedInMessage>,
      IHandleMessage<ConnectionEstablishedMessage>,
      IHandleMessage<GetAgileBoardsResponse>,
      IHandleMessage<GetAgileSprintsResponse>
   {
      private RawAgileBoard _selectedBoard;
      private RawAgileSprint _selectedSprint;
      private readonly IMessageBus _messageBus;

      public SearchBySprintField(IMessageBus messageBus)
      {
         _messageBus = messageBus;

         BoardsList = new ObservableCollection<RawAgileBoard>();
         SprintsList = new ObservableCollection<RawAgileSprint>();

         messageBus.Register(this);
      }

      private void RefreshItems()
      {
         _messageBus.Send(new GetAgileBoardsMessage());
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

            _messageBus.Send(new GetAgileSprintsMessage(SelectedBoard));

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
            return new SearchBySprintFieldControl();
         }
      }

      public ICommand ClearCommand
      {
         get
         {
            return new RelayCommand(() =>
            {
               SelectedSprint = null;
               SelectedBoard = null;
            });
         }
      }

      public string GetSearchQuery()
      {
         return string.Format("Sprint = {0}", SelectedSprint.Id);
      }

      public void Handle(ConnectionEstablishedMessage message)
      {
         RefreshItems();
      }

      public void Handle(LoggedInMessage message)
      {
         RefreshItems();
      }

      public void Handle(GetAgileSprintsResponse message)
      {
         var sprints = message.Sprints;
         DispatcherHelper.CheckBeginInvokeOnUI(() =>
         {
            SprintsList.Clear();
            foreach (var sprint in sprints.OrderBy(x => x.Name))
               SprintsList.Add(sprint);
         });
      }

      public void Handle(GetAgileBoardsResponse message)
      {
         DispatcherHelper.CheckBeginInvokeOnUI(() =>
         {
            SprintsList.Clear();
            SelectedSprint = null;
            BoardsList.Clear();
            foreach (var board in message.Boards.Where(x => x.Type == "scrum").OrderBy(x => x.Name))
               BoardsList.Add(board);
         });
      }
   }
}
