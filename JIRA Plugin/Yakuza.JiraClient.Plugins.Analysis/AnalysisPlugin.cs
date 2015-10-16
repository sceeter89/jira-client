using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Media.Imaging;
using Yakuza.JiraClient.Api;
using Yakuza.JiraClient.Api.Messages.Navigation;
using Yakuza.JiraClient.Api.Plugins;
using Yakuza.JiraClient.Plugins.Analysis.Analysis;

namespace Yakuza.JiraClient.Plugins.Analysis
{
   [Export(typeof(IJiraClientPlugin))]
   public class AnalysisPlugin : IJiraClientPlugin
   {
      private readonly EngagementChartViewModel _engagementChartViewModel = new EngagementChartViewModel();
      private readonly PivotGridViewModel _pivotViewModel = new PivotGridViewModel();

      public string PluginName
      {
         get
         {
            return "Basic charting plugin";
         }
      }

      public IEnumerable<MenuEntryDescriptor> GetMenuEntries()
      {
         yield return new MenuEntryDescriptor
         {
            Tab = MenuTab.Home,
            ButtonsGroupName = "charts",
            Buttons = new[]
               {
                  new MenuEntryButton
                  {
                     Label = "engagement",
                     OnClickCommand = _engagementChartViewModel.OpenWindowCommand,
                     Icon = new BitmapImage(new Uri(@"pack://application:,,,/JiraClient Analysis Plugin;component/Assets/Chart_Engagement.png"))
                  }
               }
         };
         yield return new MenuEntryDescriptor
         {
            Tab = MenuTab.Home,
            ButtonsGroupName = "analysis",
            Buttons = new[]
               {
                  new MenuEntryButton
                  {
                     Label = "pivot",
                     OnClickDelegate = bus => bus.Send(new ShowDocumentPaneMessage(this, "Pivot analysis",
                                                   new PivotReportingGrid {DataContext = _pivotViewModel},
                                                   new PivotReportingProperties { DataContext = _pivotViewModel})),
                     Icon = new BitmapImage(new Uri(@"pack://application:,,,/JiraClient Analysis Plugin;component/Assets/PivotTable.png"))
                  }
               }
         };
      }

      public IEnumerable<IMicroservice> GetMicroservices()
      {
         yield return _engagementChartViewModel;
         yield return _pivotViewModel;
      }
   }
}
