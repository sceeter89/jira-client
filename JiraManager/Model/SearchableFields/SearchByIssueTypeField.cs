using System;
using JiraManager.Api;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.ObjectModel;
using JiraManager.Messages.Actions.Authentication;
using GalaSoft.MvvmLight.Threading;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using System.Windows.Controls;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using JiraManager.Controls.Fields.Search;

namespace JiraManager.Model.SearchableFields
{
   public class SearchByIssueTypeField : ViewModelBase, ISearchableField
   {
      private readonly IJiraOperations _operations;
      private RawIssueType _selectedIssueType;

      public SearchByIssueTypeField(IMessenger messenger, IJiraOperations operations)
      {
         _operations = operations;

         IssueTypesList = new ObservableCollection<RawIssueType>();

         messenger.Register<LoggedInMessage>(this, async m =>
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
