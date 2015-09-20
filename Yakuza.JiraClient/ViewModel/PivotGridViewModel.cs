using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Telerik.Pivot.Core;
using System.Linq;
using Yakuza.JiraClient.Api.Messages.Actions;
using Yakuza.JiraClient.Api.Model;

namespace Yakuza.JiraClient.ViewModel
{
   public class PivotGridViewModel : ViewModelBase
   {
      private readonly IMessenger _messenger;

      public PivotGridViewModel(IMessenger messenger)
      {
         _messenger = messenger;
         _messenger.Register<NewSearchResultsAvailable>(this, RefreshPivot);
         DataSource = new LocalDataSourceProvider();
      }

      private void RefreshPivot(NewSearchResultsAvailable message)
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
