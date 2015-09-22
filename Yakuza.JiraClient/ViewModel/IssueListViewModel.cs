using GalaSoft.MvvmLight;
using Yakuza.JiraClient.Api.Model;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.Generic;
using Yakuza.JiraClient.Api.Messages.Actions;
using Telerik.Windows.Data;
using System.Linq;

namespace Yakuza.JiraClient.ViewModel
{
   public class IssueListViewModel : ViewModelBase
   {
      private QueryableCollectionView _issues;
      private readonly IMessenger _messenger;

      public IssueListViewModel(IMessenger messenger)
      {
         _messenger = messenger;
         messenger.Register<NewSearchResultsAvailable>(this, m => Issues = new QueryableCollectionView( m.SearchResults));
         messenger.Register<GetFilteredIssuesListMessage>(this, SendList);
      }

      private void SendList(GetFilteredIssuesListMessage message)
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
