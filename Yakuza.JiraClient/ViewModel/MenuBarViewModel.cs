using GalaSoft.MvvmLight;
using Yakuza.JiraClient.Api.Messages.Actions.Authentication;
using Yakuza.JiraClient.Messaging.Api;
using System.Linq;
using GalaSoft.MvvmLight.Command;
using Yakuza.JiraClient.Api.Messages.Actions;
using Yakuza.JiraClient.Api;
using Yakuza.JiraClient.Api.Messages.IO.Exports;
using Yakuza.JiraClient.Api.Messages.Navigation;
using Yakuza.JiraClient.Controls;
using System;
using Yakuza.JiraClient.Api.Messages.IO.Plugins;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using Yakuza.JiraClient.Api.Plugins;
using System.Collections.Generic;
using GalaSoft.MvvmLight.Threading;

namespace Yakuza.JiraClient.ViewModel
{
   public class MenuBarViewModel : ViewModelBase,
      IHandleMessage<LoggedInMessage>,
      IHandleMessage<LoggedOutMessage>,
      IHandleMessage<NoUpdatesAvailable>,
      IHandleMessage<NewVersionAvailable>,
      IHandleMessage<NewPluginFoundMessage>
   {
      private bool _isLoggedIn;
      private readonly IMessageBus _messageBus;
      
      public RelayCommand ShowPivotViewCommand { get; private set; }
      public RelayCommand<Button> HandleButtonClickCommand { get; private set; }

      public MenuBarViewModel(IMessageBus messageBus)
      {
         _messageBus = messageBus;
         ShowPivotViewCommand = new RelayCommand(() => _messageBus.Send(new ShowDocumentPaneMessage
         (
            this, "pivot", new PivotReportingGrid(), new PivotReportingProperties()
            )), () => _isLoggedIn);
         HandleButtonClickCommand = new RelayCommand<Button>(HandleButtonClick);

         _messageBus.Register(this);
         MenuTabs = new ObservableCollection<Tab>();
         foreach (MenuTab value in Enum.GetValues(typeof(MenuTab)))
         {
            var tab = new Tab(value.ToString());
            this.MenuTabs.Add(tab);
            _menuTabsMap[value] = tab;
         }
      }

      public void Handle(LoggedOutMessage message)
      {
         _isLoggedIn = false;
         RefreshCommands();
      }

      public void Handle(LoggedInMessage message)
      {
         _isLoggedIn = true;
         RefreshCommands();
      }

      private void RefreshCommands()
      {
         foreach (var command in GetType().GetProperties().Where(p => p.PropertyType == typeof(RelayCommand)))
         {
            ((RelayCommand)command.GetValue(this)).RaiseCanExecuteChanged();
         }
      }

      public void Handle(NoUpdatesAvailable message)
      {
         _messageBus.LogMessage("You are using latest version available.", LogLevel.Info);
      }

      public void Handle(NewVersionAvailable message)
      {
         _messageBus.LogMessage("New version is available. Visit website for download.", LogLevel.Info);
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
               var newButton = new Button
               {
                  Label = button.Label,
                  Icon = button.Icon,
                  OnClick = button.OnClick
               };
               newButton.Icon.Freeze();
               newMenuGroup.Buttons.Add(newButton);
            }
            DispatcherHelper.CheckBeginInvokeOnUI(() => tab.Groups.Add(newMenuGroup));
         }
      }

      private void HandleButtonClick(Button button)
      {
         _messageBus.LogMessage("Blablabla");
         button.OnClick(_messageBus);
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
         public Action<IMessageBus> OnClick { get; set; }
      }
   }
}
