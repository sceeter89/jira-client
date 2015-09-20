using Yakuza.JiraClient.Api;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Threading;
using System.Linq;
using GalaSoft.MvvmLight;
using System.Windows.Controls;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using Yakuza.JiraClient.Api.Model;
using Yakuza.JiraClient.Api.Messages.Actions.Authentication;
using Yakuza.JiraClient.Api.Messages.Status;

namespace Yakuza.JiraClient.IssueFields.Search
{
   public class ComboBoxSearchField : ViewModelBase, ISearchableField
   {
      private readonly IJiraOperations _operations;
      private RawIssueType _selectedIssueType;

      public ComboBoxSearchField(IMessenger messenger, IJiraOperations operations)
      {
         _operations = operations;

         IssueTypesList = new ObservableCollection<RawIssueType>();

         messenger.Register<LoggedInMessage>(this, async m => await RefreshItems());

         messenger.Register<ConnectionEstablishedMessage>(this, async m => await RefreshItems());
      }

      private async System.Threading.Tasks.Task RefreshItems()
      {
         var issueTypes = await _operations.GetIssueTypes();
         if (issueTypes == null)
            return;
         DispatcherHelper.CheckBeginInvokeOnUI(() =>
         {
            IssueTypesList.Clear();
            foreach (var issueType in issueTypes.OrderBy(x => x.Name))
               IssueTypesList.Add(issueType);
         });
      }

      public ObservableCollection<RawIssueType> IssueTypesList { get; private set; }

      public bool IsFilled
      {
         get
         {
            return SelectedIssueType != null;
         }
      }

      public RawIssueType SelectedIssueType
      {
         get { return _selectedIssueType; }
         set
         {
            _selectedIssueType = value;

            RaisePropertyChanged();
         }
      }

      public UserControl EditControl
      {
         get
         {
            return new SearchByIssueTypeFieldControl();
         }
      }

      public ICommand ClearCommand
      {
         get
         {
            return new RelayCommand(() => SelectedIssueType = null);
         }
      }

      public string GetSearchQuery()
      {
         return string.Format("IssueType = '{0}'", SelectedIssueType.Name);
      }
   }
}
