using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Reflection;
using System.Windows.Media.Imaging;
using Yakuza.JiraClient.Api;
using Yakuza.JiraClient.Api.Messages.Actions;
using Yakuza.JiraClient.Api.Messages.IO.Exports;
using Yakuza.JiraClient.Api.Messages.Navigation;
using Yakuza.JiraClient.Api.Plugins;
using Yakuza.JiraClient.Plugins.Diagnostics.Controls;

namespace Yakuza.JiraClient.Plugins.Diagnostics
{
   [Export(typeof(IJiraClientPlugin))]
   public class DiagnosticsPlugin : IJiraClientPlugin
   {
      private const string WebSiteAddress = "https://github.com/sceeter89/jira-client";
      private const string ReportIssueSiteAddress = "https://github.com/sceeter89/jira-client/issues/new";

      private readonly PluginsViewModel _pluginsViewModel = new PluginsViewModel();
      private readonly PerformanceOverviewViewModel _performanceOverviewViewModel = new PerformanceOverviewViewModel();

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
                     OnClick = bus =>bus.Send(new SaveLogOutputToFileMessage())
                  },
                  new MenuEntryButton
                  {
                     Label = "plugins",
                     Icon = new BitmapImage(new Uri(@"pack://application:,,,/JiraClient Diagnostics Plugin;component/Assets/PluginsIcon.png")),
                     OnClick = bus => bus.Send(new ShowDocumentPaneMessage(this, "Plugins",
                                                   new PluginView { DataContext = _pluginsViewModel },
                                                   new PluginsListView {DataContext = _pluginsViewModel}))
                  },
                  new MenuEntryButton
                  {
                     Label = "performance",
                     Icon = new BitmapImage(new Uri(@"pack://application:,,,/JiraClient Diagnostics Plugin;component/Assets/PerformanceIcon.png")),
                     OnClick = bus => bus.Send(new ShowDocumentPaneMessage(this, "Performance",
                                                   new PerformanceOverview { DataContext = _performanceOverviewViewModel }))
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
                     OnClick = _ => System.Diagnostics.Process.Start(WebSiteAddress)
                  },
                  new MenuEntryButton
                  {
                     Label = "report",
                     Icon = new BitmapImage(new Uri(@"pack://application:,,,/JiraClient Diagnostics Plugin;component/Assets/ReportIssueIcon.png")),
                     OnClick = _ => System.Diagnostics.Process.Start(ReportIssueSiteAddress)
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
                     OnClick = bus => bus.Send(new CheckForUpdatesMessage(Assembly.GetEntryAssembly().GetName().Version))
                  }
               }
         };
      }

      public IEnumerable<IMicroservice> GetMicroservices()
      {
         yield return _pluginsViewModel;
         yield return _performanceOverviewViewModel;
      }
   }
}
