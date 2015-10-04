using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Media.Imaging;
using Yakuza.JiraClient.Api;
using Yakuza.JiraClient.Api.Messages.Navigation;
using Yakuza.JiraClient.Api.Plugins;
using Yakuza.JiraClient.Messaging.Api;
using Yakuza.JiraClient.Plugins.Analysis.Analysis;
using Yakuza.JiraClient.Plugins.Analysis.Charts;

namespace Yakuza.JiraClient.Plugins.Analysis
{
   [Export(typeof(IJiraClientPlugin))]
   public class AnalysisPlugin : IJiraClientPlugin
   {
      private readonly ChartingDisplayViewModel _viewModel = new ChartingDisplayViewModel();
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
                     OnClick = new Action<IMessageBus>(bus => bus.Send(new ShowDocumentPaneMessage(this, "Chart - Engagement",new EngagementChartControl { DataContext = _viewModel }))),
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
                     OnClick = bus => bus.Send(new ShowDocumentPaneMessage(this, "Pivot analysis", 
                                                   new PivotReportingGrid {DataContext = _pivotViewModel},
                                                   new PivotReportingProperties { DataContext = _pivotViewModel})),
                     Icon = new BitmapImage(new Uri(@"pack://application:,,,/JiraClient Analysis Plugin;component/Assets/PivotTable.png"))
                  }
               }
         };
      }

      public IEnumerable<IMicroservice> GetMicroservices()
      {
         yield return _viewModel;
         yield return _pivotViewModel;
      }
   }
}
