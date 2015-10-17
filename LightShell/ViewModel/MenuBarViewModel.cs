using GalaSoft.MvvmLight;
using LightShell.Messaging.Api;
using GalaSoft.MvvmLight.Command;
using LightShell.Api;
using System;
using LightShell.Api.Messages.IO.Plugins;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using LightShell.Api.Plugins;
using System.Collections.Generic;
using GalaSoft.MvvmLight.Threading;
using LightShell.InternalMessages.UI;
using System.Windows.Input;

namespace LightShell.ViewModel
{
   internal class MenuBarViewModel : ViewModelBase,
      IHandleMessage<NewPluginFoundMessage>
   {
      private readonly IMessageBus _messageBus;
      
      public MenuBarViewModel(IMessageBus messageBus)
      {
         _messageBus = messageBus;

         _messageBus.Register(this);
         MenuTabs = new ObservableCollection<Tab>();
         foreach (MenuTab value in Enum.GetValues(typeof(MenuTab)))
         {
            var tab = new Tab(value.ToString());
            this.MenuTabs.Add(tab);
            _menuTabsMap[value] = tab;
         }

         messageBus.Send(new ViewModelInitializedMessage(this.GetType()));
      }

      public void Handle(NewPluginFoundMessage message)
      {
         _messageBus.LogMessage(LogLevel.Debug, "Loading UI for plugin: {0}...", message.PluginDescription.PluginName);
         var menuEntries = message.PluginDescription.GetMenuEntries();
         if (menuEntries == null)
            return;

         foreach (var descriptor in menuEntries)
         {
            var tab = _menuTabsMap[descriptor.Tab];
            var newMenuGroup = new Group(descriptor.ButtonsGroupName);

            foreach (var button in descriptor.Buttons)
            {
               if (string.IsNullOrWhiteSpace(button.Label))
               {
                  _messageBus.LogMessage(LogLevel.Warning, "Button {0}.{1}.{2} has no label defined. Skipping...", descriptor.Tab, descriptor.ButtonsGroupName, button.Label);
                  continue;
               }
               if(button.OnClickCommand == null && button.OnClickDelegate == null)
               {
                  _messageBus.LogMessage(LogLevel.Warning, "Button {0}.{1}.{2} does not define any action. Skipping...", descriptor.Tab, descriptor.ButtonsGroupName, button.Label);
                  continue;
               }

               var newButton = new Button
               {
                  Label = button.Label,
                  Icon = button.Icon,
                  OnClick = button.OnClickCommand != null ? button.OnClickCommand : new RelayCommand(() => button.OnClickDelegate(_messageBus))
               };
               newButton.Icon.Freeze();
               newMenuGroup.Buttons.Add(newButton);
            }
            DispatcherHelper.CheckBeginInvokeOnUI(() => tab.Groups.Add(newMenuGroup));
         }
      }

      private readonly IDictionary<MenuTab, Tab> _menuTabsMap = new Dictionary<MenuTab, Tab>();
      public ObservableCollection<Tab> MenuTabs { get; private set; }

      public class Tab
      {
         public Tab(string title)
         {
            Title = title;
            Groups = new ObservableCollection<Group>();
         }
         public string Title { get; private set; }
         public ObservableCollection<Group> Groups { get; private set; }
      }

      public class Group
      {
         public Group(string groupName)
         {
            GroupName = groupName;
            Buttons = new ObservableCollection<Button>();
         }
         public string GroupName { get; private set; }
         public ObservableCollection<Button> Buttons { get; private set; }
      }

      public class Button
      {
         public string Label { get; set; }
         public BitmapImage Icon { get; set; }
         public ICommand OnClick { get; set; }
      }
   }
}
