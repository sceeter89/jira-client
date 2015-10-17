using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Reflection;
using System.Windows.Media.Imaging;
using LightShell.Plugin.Jira.Api.Messages.IO.Exports;
using LightShell.Plugin.Jira.Diagnostics.Controls;
using LightShell.Api.Plugins;
using LightShell.Api.Messages.Navigation;
using LightShell.Api.Messages.IO.Exports;
using LightShell.Plugin.Jira.Api.Messages.Actions;
using LightShell.Api;

namespace Yakuza.JiraClient.Plugins.Diagnostics
{
   [Export(typeof(ILightShellPlugin))]
   public class DiagnosticsPlugin : ILightShellPlugin
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
                     OnClickDelegate = bus =>bus.Send(new SaveLogOutputToFileMessage())
                  },
                  new MenuEntryButton
                  {
                     Label = "plugins",
                     Icon = new BitmapImage(new Uri(@"pack://application:,,,/JiraClient Diagnostics Plugin;component/Assets/PluginsIcon.png")),
                     OnClickDelegate = bus => bus.Send(new ShowDocumentPaneMessage(this, "Plugins",
                                                   new PluginView { DataContext = _pluginsViewModel },
                                                   new PluginsListView {DataContext = _pluginsViewModel}))
                  },
                  new MenuEntryButton
                  {
                     Label = "performance",
                     Icon = new BitmapImage(new Uri(@"pack://application:,,,/JiraClient Diagnostics Plugin;component/Assets/PerformanceIcon.png")),
                     OnClickDelegate = bus => bus.Send(new ShowDocumentPaneMessage(this, "Performance",
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
                     OnClickDelegate = _ => System.Diagnostics.Process.Start(WebSiteAddress)
                  },
                  new MenuEntryButton
                  {
                     Label = "report",
                     Icon = new BitmapImage(new Uri(@"pack://application:,,,/JiraClient Diagnostics Plugin;component/Assets/ReportIssueIcon.png")),
                     OnClickDelegate = _ => System.Diagnostics.Process.Start(ReportIssueSiteAddress)
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
                     OnClickDelegate = bus => bus.Send(new CheckForUpdatesMessage(Assembly.GetEntryAssembly().GetName().Version))
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
