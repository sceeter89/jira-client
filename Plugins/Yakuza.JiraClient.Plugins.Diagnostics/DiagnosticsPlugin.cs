using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Reflection;
using System.Windows.Media.Imaging;
using Yakuza.JiraClient.Api;
using Yakuza.JiraClient.Api.Messages.Actions;
using Yakuza.JiraClient.Api.Messages.IO.Exports;
using Yakuza.JiraClient.Api.Plugins;
using Yakuza.JiraClient.Messaging.Api;

namespace Yakuza.JiraClient.Plugins.Diagnostics
{
   [Export(typeof(IJiraClientPlugin))]
   public class DiagnosticsPlugin : IJiraClientPlugin
   {
      private const string WebSiteAddress = "https://github.com/sceeter89/jira-client";
      private const string ReportIssueSiteAddress = "https://github.com/sceeter89/jira-client/issues/new";

      public string PluginName
      {
         get
         {
            return "Diagnostics plugin";
         }
      }

      public IEnumerable<MenuEntryDescriptor> GetMenuEntries()
      {
         yield return new MenuEntryDescriptor
         {
            Tab = MenuTab.Support,
            ButtonsGroupName = "diagnostics",
            Buttons = new[]
               {
                  new MenuEntryButton
                  {
                     Label = "export log",
                     Icon = new BitmapImage(new Uri(@"pack://application:,,,/JiraClient Diagnostics Plugin;component/Assets/SaveIcon.png")),
                     OnClick = SendSaveLogRequest
                  }
               }
         };
         yield return new MenuEntryDescriptor
         {
            Tab = MenuTab.Support,
            ButtonsGroupName = "help",
            Buttons = new[]
               {
                  new MenuEntryButton
                  {
                     Label = "website",
                     Icon = new BitmapImage(new Uri(@"pack://application:,,,/JiraClient Diagnostics Plugin;component/Assets/WebsiteIcon.png")),
                     OnClick = new Action<IMessageBus>(_ => System.Diagnostics.Process.Start(WebSiteAddress))
                  },
                  new MenuEntryButton
                  {
                     Label = "report",
                     Icon = new BitmapImage(new Uri(@"pack://application:,,,/JiraClient Diagnostics Plugin;component/Assets/ReportIssueIcon.png")),
                     OnClick = new Action<IMessageBus>(_ => System.Diagnostics.Process.Start(ReportIssueSiteAddress))
                  }
               }
         };
         yield return new MenuEntryDescriptor
         {
            Tab = MenuTab.Support,
            ButtonsGroupName = "updates",
            Buttons = new[]
               {
                  new MenuEntryButton
                  {
                     Label = "check",
                     Icon = new BitmapImage(new Uri(@"pack://application:,,,/JiraClient Diagnostics Plugin;component/Assets/CheckForUpdatesIcon.png")),
                     OnClick = SendUpdatesCheckRequest
                  }
               }
         };
      }

      private void SendUpdatesCheckRequest(IMessageBus bus)
      {
         bus.Send(new CheckForUpdatesMessage(Assembly.GetEntryAssembly().GetName().Version));
      }

      private void SendSaveLogRequest(IMessageBus bus)
      {
         bus.Send(new SaveLogOutputToFileMessage());
      }

      public IEnumerable<IMicroservice> GetMicroservices()
      {
         return null;
      }
   }
}
