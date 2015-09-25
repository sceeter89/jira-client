using GalaSoft.MvvmLight;
using Telerik.Pivot.Core;
using System.Linq;
using Yakuza.JiraClient.Api.Model;
using Yakuza.JiraClient.Messaging.Api;
using Yakuza.JiraClient.Api.Messages.IO.Jira;

namespace Yakuza.JiraClient.ViewModel
{
   public class PivotGridViewModel : ViewModelBase,
      IHandleMessage<SearchForIssuesResponse>
   {
      private readonly IMessageBus _messenger;

      public PivotGridViewModel(IMessageBus messenger)
      {
         _messenger = messenger;
         DataSource = new LocalDataSourceProvider();

         _messenger.Register(this);
      }
      
      public void Handle(SearchForIssuesResponse message)
      {
         using (DataSource.DeferRefresh())
         {
            DataSource.ItemsSource = message.SearchResults
               .Select(x => new PivotJiraIssue(x)).ToList();
         }
      }

      public LocalDataSourceProvider DataSource { get; private set; }
   }
}
