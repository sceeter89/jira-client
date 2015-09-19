using GalaSoft.MvvmLight;
using Yakuza.JiraClient.Api.Model;
using System.Collections.ObjectModel;

namespace Yakuza.JiraClient.ViewModel
{
   public class IssueListViewModel : ViewModelBase
   {
      private readonly SearchIssuesViewModel _searchIssuesViewModel;

      public IssueListViewModel(SearchIssuesViewModel searchIssuesViewModel)
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
