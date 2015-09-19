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

namespace Yakuza.JiraClient.IssueFields.Search
{
   public class SearchByPriorityField : ViewModelBase, ISearchableField
   {
      private readonly IJiraOperations _operations;
      private RawPriority _selectedIssueType;

      public SearchByPriorityField(IMessenger messenger, IJiraOperations operations)
      {
         _operations = operations;

         PrioritiesList = new ObservableCollection<RawPriority>();

         messenger.Register<LoggedInMessage>(this, async m =>
         {
            var priorities = await _operations.GetPrioritiesList();
            if (priorities == null)
               return;
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
               PrioritiesList.Clear();
               foreach (var priority in priorities.OrderBy(x => x.Name))
                  PrioritiesList.Add(priority);
            });
         });
      }

      public ObservableCollection<RawPriority> PrioritiesList { get; private set; }

      public bool IsFilled
      {
         get
         {
            return SelectedPriority != null;
         }
      }

      public RawPriority SelectedPriority
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
            return new SearchByPriorityFieldControl();
         }
      }

      public ICommand ClearCommand
      {
         get
         {
            return new RelayCommand(() => SelectedPriority = null);
         }
      }

      public string GetSearchQuery()
      {
         return string.Format("priority = '{0}'", SelectedPriority.Name);
      }
   }
}
