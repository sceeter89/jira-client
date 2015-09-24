using GalaSoft.MvvmLight;
using Yakuza.JiraClient.Api.Model;
using GalaSoft.MvvmLight.Messaging;
using Yakuza.JiraClient.Api.Messages.Actions;
using Telerik.Windows.Data;
using System.Linq;
using Yakuza.JiraClient.Messaging.Api;
using System;

namespace Yakuza.JiraClient.ViewModel
{
   public class IssueListViewModel : ViewModelBase,
      IHandleMessage<NewSearchResultsAvailableMessage>,
      IHandleMessage<GetFilteredIssuesListMessage>
   {
      private QueryableCollectionView _issues;
      private readonly IMessageBus _messenger;

      public IssueListViewModel(IMessageBus messenger)
      {
         _messenger = messenger;
         messenger.Register(this);
      }
      
      public void Handle(NewSearchResultsAvailableMessage message)
      {
         Issues = new QueryableCollectionView(message.SearchResults);
      }

      public void Handle(GetFilteredIssuesListMessage message)
      {
         _messenger.Send(new FilteredIssuesListMessage(Issues.Cast<JiraIssue>()));
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
   }
}
