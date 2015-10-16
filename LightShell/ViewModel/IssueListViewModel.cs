using GalaSoft.MvvmLight;
using LightShell.Api.Model;
using LightShell.Api.Messages.Actions;
using Telerik.Windows.Data;
using System.Linq;
using LightShell.Messaging.Api;
using LightShell.Api.Messages.IO.Jira;
using GalaSoft.MvvmLight.Command;
using LightShell.Api.Messages.Navigation;
using LightShell.Controls.Panes;

namespace LightShell.ViewModel
{
   internal class IssueListViewModel : ViewModelBase,
      IHandleMessage<SearchForIssuesResponse>,
      IHandleMessage<GetFilteredIssuesListMessage>
   {
      private QueryableCollectionView _issues;
      private readonly IMessageBus _messenger;

      public IssueListViewModel(IMessageBus messenger)
      {
         _messenger = messenger;
         messenger.Register(this);
      }

      public void Handle(SearchForIssuesResponse message)
      {
         Issues = new QueryableCollectionView(message.SearchResults);
      }

      public void Handle(GetFilteredIssuesListMessage message)
      {
         _messenger.Send(new FilteredIssuesListMessage(Issues == null ? Enumerable.Empty<JiraIssue>() : Issues.Cast<JiraIssue>()));
      }

      public QueryableCollectionView Issues
      {
         get { return _issues; }
         set
         {
            _issues = value;
            RaisePropertyChanged();
         }
      }

      public JiraIssue SelectedIssue { get; set; }
      private RelayCommand _rowDoubleClickedCommand;
      public RelayCommand RowDoubleClickedCommand
      {
         get
         {
            if (_rowDoubleClickedCommand == null)
               _rowDoubleClickedCommand = new RelayCommand(ShowRowDetails);
            return _rowDoubleClickedCommand;
         }
      }

      private void ShowRowDetails()
      {
         if (SelectedIssue == null)
            return;

         _messenger.Send(new ShowDocumentPaneMessage(this, SelectedIssue.Key, new IssueDetails { DataContext = SelectedIssue }));
      }
   }
}
