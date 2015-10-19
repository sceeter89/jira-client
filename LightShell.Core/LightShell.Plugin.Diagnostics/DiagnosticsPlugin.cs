using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Media.Imaging;
using LightShell.Plugin.Diagnostics.Controls;
using LightShell.Api.Plugins;
using LightShell.Api.Messages.Navigation;
using LightShell.Api.Messages.IO.Exports;
using LightShell.Api;

namespace LightShell.Plugin.Diagnostics
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
            return "LightShell - diagnostics";
         }
      }

      public IEnumerable<MenuEntryDescriptor> GetMenuEntries()
      {
         yield return new MenuEntryDescriptor
         {
            Tab = "support",
            ButtonsGroupName = "diagnostics",
            Buttons = new[]
               {
                  new MenuEntryButton
                  {
                     Label = "export log",
                     Icon = new BitmapImage(new Uri(@"pack://application:,,,/LightShell.Plugin.Diagnostics;component/Assets/SaveIcon.png")),
                     OnClickDelegate = bus =>bus.Send(new SaveLogOutputToFileMessage())
                  },
                  new MenuEntryButton
                  {
                     Label = "plugins",
                     Icon = new BitmapImage(new Uri(@"pack://application:,,,/LightShell.Plugin.Diagnostics;component/Assets/PluginsIcon.png")),
                     OnClickDelegate = bus => bus.Send(new ShowDocumentPaneMessage(this, "Plugins",
                                                   new PluginView { DataContext = _pluginsViewModel },
                                                   new PluginsListView {DataContext = _pluginsViewModel}))
                  },
                  new MenuEntryButton
                  {
                     Label = "performance",
                     Icon = new BitmapImage(new Uri(@"pack://application:,,,/LightShell.Plugin.Diagnostics;component/Assets/PerformanceIcon.png")),
                     OnClickDelegate = bus => bus.Send(new ShowDocumentPaneMessage(this, "Performance",
                                                   new PerformanceOverview { DataContext = _performanceOverviewViewModel }))
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
