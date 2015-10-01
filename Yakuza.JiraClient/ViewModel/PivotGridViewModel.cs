using GalaSoft.MvvmLight;
using Telerik.Pivot.Core;
using System.Linq;
using Yakuza.JiraClient.Api.Model;
using Yakuza.JiraClient.Messaging.Api;
using Yakuza.JiraClient.Api.Messages.IO.Jira;
using Yakuza.JiraClient.Api.Messages.Actions;
using System.Collections.Generic;

namespace Yakuza.JiraClient.ViewModel
{
   public class PivotGridViewModel : ViewModelBase,
      IHandleMessage<SearchForIssuesResponse>,
      IHandleMessage<CurrentSearchResultsResponse>
   {
      private readonly IMessageBus _messenger;

      public PivotGridViewModel(IMessageBus messenger)
      {
         _messenger = messenger;
         DataSource = new LocalDataSourceProvider();

         _messenger.Register(this);

         _messenger.Send(new CurrentSearchResultsMessage());
      }

      public void Handle(SearchForIssuesResponse message)
      {
         LoadSearchResults(message.SearchResults);
      }

      public void Handle(CurrentSearchResultsResponse message)
      {
         LoadSearchResults(message.SearchResults);
      }

      private void LoadSearchResults(ICollection<JiraIssue> searchResults)
      {
         using (DataSource.DeferRefresh())
         {
            DataSource.ItemsSource = searchResults
               .Select(x => new PivotJiraIssue(x)).ToList();
         }
      }

      public LocalDataSourceProvider DataSource { get; private set; }
   }
}
