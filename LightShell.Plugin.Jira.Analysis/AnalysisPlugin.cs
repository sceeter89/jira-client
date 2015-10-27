using LightShell.Api;
using LightShell.Api.Plugins;
using LightShell.Plugin.Jira.Analysis.Analysis;
using LightShell.Plugin.Jira.Analysis.Charts;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Media.Imaging;

namespace LightShell.Plugin.Jira.Analysis
{
   [Export(typeof(ILightShellPlugin))]
   public class AnalysisPlugin : ILightShellPlugin
   {
      private readonly EngagementChartViewModel _engagementChartViewModel = new EngagementChartViewModel();
      private readonly PivotGridViewModel _pivotViewModel = new PivotGridViewModel();

      public string PluginName
      {
         get
         {
            return "JIRA Plugin - analysis";
         }
      }

      public IEnumerable<MenuEntryDescriptor> GetMenuEntries()
      {
         yield return new MenuEntryDescriptor
         {
            Tab = "home",
            ButtonsGroupName = "charts",
            Buttons = new[]
               {
                  new MenuEntryButton
                  {
                     Label = "engagement",
                     OnClickCommand = _engagementChartViewModel.OpenWindowCommand,
                     Icon = new BitmapImage(new Uri(@"pack://application:,,,/Jira Analysis Plugin;component/Assets/Chart_Engagement.png"))
                  }
               }
         };
         yield return new MenuEntryDescriptor
         {
            Tab = "home",
            ButtonsGroupName = "analysis",
            Buttons = new[]
               {
                  new MenuEntryButton
                  {
                     Label = "pivot",
                     OnClickCommand = _pivotViewModel.OpenWindowCommand,
                     Icon = new BitmapImage(new Uri(@"pack://application:,,,/Jira Analysis Plugin;component/Assets/PivotTable.png"))
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
