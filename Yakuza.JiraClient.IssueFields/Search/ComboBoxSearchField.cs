using Yakuza.JiraClient.Api;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Threading;
using System.Linq;
using GalaSoft.MvvmLight;
using System.Windows.Controls;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using Yakuza.JiraClient.Api.Messages.Actions.Authentication;
using Yakuza.JiraClient.Api.Messages.Status;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace Yakuza.JiraClient.IssueFields.Search
{
   public class ComboBoxSearchField<T> : ViewModelBase, ISearchableField
      where T : class
   {
      private PickUpItem<T> _selectedItem;
      private readonly Func<Task<IEnumerable<T>>> _itemsGetter;
      private readonly Func<T, string> _displayNameGetter;
      private readonly Func<T, string> _queryValueGetter;
      private readonly string _queryFieldName;

      public ComboBoxSearchField(IMessenger messenger,
                                 Func<Task<IEnumerable<T>>> itemsGetter,
                                 Func<T, string> displayNameGetter,
                                 Func<T, string> queryValueGetter,
                                 string queryFieldName,
                                 string label)
      {
         Items = new ObservableCollection<PickUpItem<T>>();
         Label = label;
         _itemsGetter = itemsGetter;
         _displayNameGetter = displayNameGetter;
         _queryValueGetter = queryValueGetter;
         _queryFieldName = queryFieldName;
         messenger.Register<LoggedInMessage>(this, async m => await RefreshItems());
         messenger.Register<ConnectionEstablishedMessage>(this, async m => await RefreshItems());
      }

      private async Task RefreshItems()
      {
         var items = await _itemsGetter();
         if (items == null)
            return;
         DispatcherHelper.CheckBeginInvokeOnUI(() =>
         {
            Items.Clear();
            foreach (var item in items.Select(x => new PickUpItem<T>
            {
               IsSelected = false,
               Item = x,
               Name = _displayNameGetter(x)
            }).OrderBy(x => x.Name))
               Items.Add(item);
         });
      }

      public ObservableCollection<PickUpItem<T>> Items { get; private set; }
      public string Label { get; private set; }

      public bool IsFilled
      {
         get
         {
            return SelectedItem != null;
         }
      }

      public PickUpItem<T> SelectedItem
      {
         get { return _selectedItem; }
         set
         {
            _selectedItem = value;

            RaisePropertyChanged();
         }
      }

      public UserControl EditControl
      {
         get
         {
            return new ComboBoxSearchFieldControl();
         }
      }

      public ICommand ClearCommand
      {
         get
         {
            return new RelayCommand(() => SelectedItem = null);
         }
      }

      public string GetSearchQuery()
      {
         return string.Format("{0} = '{1}'", _queryFieldName, _queryValueGetter(SelectedItem.Item));
      }

      public class PickUpItem<T>
      {
         public T Item { get; internal set; }
         public string Name { get; internal set; }
         public bool IsSelected { get; internal set; }
      }
   }
}
