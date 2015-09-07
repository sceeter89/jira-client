using GalaSoft.MvvmLight;
using JiraManager.Model;
using System.Collections.ObjectModel;

namespace JiraManager.ViewModel
{
   public class CardsPrintingViewModel : ViewModelBase
   {
      private readonly SearchIssuesViewModel _searchIssuesViewModel;

      public CardsPrintingViewModel(SearchIssuesViewModel searchIssuesViewModel)
      {
         _searchIssuesViewModel = searchIssuesViewModel;
      }

      public ObservableCollection<JiraIssue> Issues
      {
         get
         {
            return _searchIssuesViewModel.FoundIssues;
         }
      }
   }
}
