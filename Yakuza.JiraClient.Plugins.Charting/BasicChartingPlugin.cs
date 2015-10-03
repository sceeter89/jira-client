﻿using System;
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
                     OnClickCommand = _viewModel.ShowEngagementChartCommand,
                     Icon = new BitmapImage(new Uri(@"pack://application:,,,/JiraClient Charting Plugin;component/Assets/Chart_Engagement.png"))
                  }
               }
         };
      }

      public IEnumerable<IMicroservice> GetMicroservices()
      {
         yield return _viewModel;
      }
   }
}