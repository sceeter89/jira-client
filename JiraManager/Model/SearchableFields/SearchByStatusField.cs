using System;
using Yakuza.JiraClient.Api;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.ObjectModel;
using Yakuza.JiraClient.Messages.Actions.Authentication;
using GalaSoft.MvvmLight.Threading;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using System.Windows.Controls;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using Yakuza.JiraClient.Controls.Fields.Search;

namespace Yakuza.JiraClient.Model.SearchableFields
{
   public class SearchByStatusField : ViewModelBase, ISearchableField
   {
      private readonly IJiraOperations _operations;
      private RawStatus _selectedStatus;

      public SearchByStatusField(IMessenger messenger, IJiraOperations operations)
      {
         _operations = operations;

         StatusesList = new ObservableCollection<RawStatus>();

         messenger.Register<LoggedInMessage>(this, async m =>
         {
            var statuses = await _operations.GetStatusesList();
            if (statuses == null)
               return;
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
               StatusesList.Clear();
               foreach (var status in statuses.OrderBy(x => x.Name))
                  StatusesList.Add(status);
            });
         });
      }

      public ObservableCollection<RawStatus> StatusesList { get; private set; }

      public bool IsFilled
      {
         get
         {
            return SelectedStatus != null;
         }
      }

      public RawStatus SelectedStatus
      {
         get { return _selectedStatus; }
         set
         {
            _selectedStatus = value;

            RaisePropertyChanged();
         }
      }

      public UserControl EditControl
      {
         get
         {
            return new SearchByStatusFieldControl();
         }
      }

      public ICommand ClearCommand
      {
         get
         {
            return new RelayCommand(() => SelectedStatus = null);
         }
      }

      public string GetSearchQuery()
      {
         return string.Format("status = '{0}'", SelectedStatus.Name);
      }
   }
}
