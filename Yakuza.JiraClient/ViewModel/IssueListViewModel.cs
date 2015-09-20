using GalaSoft.MvvmLight;
using Yakuza.JiraClient.Api.Model;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.Generic;
using Yakuza.JiraClient.Api.Messages.Actions;

namespace Yakuza.JiraClient.ViewModel
{
   public class IssueListViewModel : ViewModelBase
   {
      private IList<JiraIssue> _issues;

      public IssueListViewModel(IMessenger messenger)
      {
         messenger.Register<NewSearchResultsAvailable>(this, m => Issues = m.SearchResults);
      }

      public IList<JiraIssue> Issues
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
