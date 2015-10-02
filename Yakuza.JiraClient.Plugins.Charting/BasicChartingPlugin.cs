using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Media.Imaging;
using Yakuza.JiraClient.Api;
using Yakuza.JiraClient.Api.Plugins;

namespace Yakuza.JiraClient.Plugins.Charting
{
   [Export(typeof(IJiraClientPlugin))]
   public class BasicChartingPlugin : IJiraClientPlugin
   {
      private readonly ChartingDisplayViewModel _viewModel = new ChartingDisplayViewModel();

      public MenuEntryDescriptor MenuEntryDescriptor
      {
         get
         {
            return new MenuEntryDescriptor
            {
               Tab = MenuTab.Home,
               ButtonsGroupName = "charts",
               Buttons = new[]
               {
                  new MenuEntryButton
                  {
                     Label = "engagement",
                     OnClickCommand = _viewModel.ShowEngagementChartCommand,
                     Icon = new BitmapImage(new Uri(@"pack://application:,,,/Assets/Chart_Engagement.png"))
                  }
               }
            };
         }
      }

      public IEnumerable<IMicroservice> Microservices
      {
         get
         {
            return new[] { _viewModel };
         }
      }

      public string PluginName
      {
         get
         {
            return "Basic charting plugin";
         }
      }
   }
}
