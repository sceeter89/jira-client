using LightShell.Api;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Threading;
using System.Linq;
using GalaSoft.MvvmLight;
using System.Windows.Controls;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using LightShell.Messaging.Api;
using LightShell.Plugin.Jira.Api.Messages.Actions.Authentication;
using LightShell.Plugin.Jira.Api.Messages.Status;

namespace LightShell.Plugin.Jira.Controls.SearchFields
{
   public class ComboBoxSearchField<TItemType, TRequestType, TResponseType> : ViewModelBase, ISearchableField,
      IHandleMessage<LoggedInMessage>,
      IHandleMessage<ConnectionEstablishedMessage>
      where TItemType : class
      where TRequestType : IMessage, new()
      where TResponseType : IMessage
   {
      private PickUpItem<TItemType> _selectedItem;
      private readonly Func<TResponseType, IEnumerable<TItemType>> _itemsGetter;
      private readonly Func<TItemType, string> _displayNameGetter;
      private readonly Func<TItemType, string> _queryValueGetter;
      private readonly string _queryFieldName;
      private readonly IMessageBus _messageBus;

      public ComboBoxSearchField(IMessageBus messageBus,
                                 Func<TResponseType, IEnumerable<TItemType>> itemsGetter,
                                 Func<TItemType, string> displayNameGetter,
                                 Func<TItemType, string> queryValueGetter,
                                 string queryFieldName,
                                 string label)
      {
         Items = new ObservableCollection<PickUpItem<TItemType>>();
         Label = label;
         _itemsGetter = itemsGetter;
         _displayNameGetter = displayNameGetter;
         _queryValueGetter = queryValueGetter;
         _queryFieldName = queryFieldName;

         _messageBus = messageBus;
         _messageBus.Listen<TResponseType>(m =>
         {
            var items = _itemsGetter(m);
            if (items == null)
               return;
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
               Items.Clear();
               foreach (var item in items.Select(x => new PickUpItem<TItemType>
               {
                  IsSelected = false,
                  Item = x,
                  Name = _displayNameGetter(x)
               }).OrderBy(x => x.Name))
                  Items.Add(item);
            });
         });
         messageBus.Register(this);
      }
      
      public ObservableCollection<PickUpItem<TItemType>> Items { get; private set; }
      public string Label { get; private set; }

      public bool IsFilled
      {
         get
         {
            return SelectedItem != null;
         }
      }

      public PickUpItem<TItemType> SelectedItem
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

      public void Handle(LoggedInMessage message)
      {
         _messageBus.Send(new TRequestType());
      }

      public void Handle(ConnectionEstablishedMessage message)
      {
         _messageBus.Send(new TRequestType());
      }

      public class PickUpItem<T>
      {
         public T Item { get; internal set; }
         public string Name { get; internal set; }
         public bool IsSelected { get; internal set; }
      }
   }
}
