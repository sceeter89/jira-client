using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Yakuza.JiraClient.Messages.Actions;
using Telerik.Pivot.Core;
using System.Linq;
using Yakuza.JiraClient.Model;

namespace Yakuza.JiraClient.ViewModel
{
   public class PivotGridViewModel : ViewModelBase
   {
      private readonly IMessenger _messenger;
      private readonly SearchIssuesViewModel _searchIssuesViewModel;

      public PivotGridViewModel(IMessenger messenger, SearchIssuesViewModel searchIssuesViewModel)
      {
         _searchIssuesViewModel = searchIssuesViewModel;
         _messenger = messenger;
         _messenger.Register<NewSearchResultsAvailable>(this, RefreshPivot);
         DataSource = new LocalDataSourceProvider();
      }

      private void RefreshPivot(NewSearchResultsAvailable obj)
      {
         using (DataSource.DeferRefresh())
         {
            DataSource.ItemsSource = _searchIssuesViewModel.FoundIssues
               .Select(x => new PivotJiraIssue(x)).ToList();
         }
      }

      public LocalDataSourceProvider DataSource { get; private set; }
   }
}
