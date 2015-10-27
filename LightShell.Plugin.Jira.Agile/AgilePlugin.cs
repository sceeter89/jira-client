using LightShell.Api;
using LightShell.Api.Plugins;
using LightShell.Plugin.Jira.Agile.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Media.Imaging;

namespace LightShell.Plugin.Jira.Agile
{
   [Export(typeof(ILightShellPlugin))]
   public class AgilePlugin : ILightShellPlugin
   {
      private readonly CardsPrintingHandler _cardsPrintingHandler = new CardsPrintingHandler();
      private readonly BurnDownChartViewModel _burndownViewModel = new BurnDownChartViewModel();

      public string PluginName
      {
         get
         {
            return "JIRA Plugin - agile";
         }
      }

      public IEnumerable<MenuEntryDescriptor> GetMenuEntries()
      {
         yield return new MenuEntryDescriptor
         {
            ButtonsGroupName = "agile",
            Tab = "home",
            Buttons = new[]
            {
               new MenuEntryButton
               {
                  Label = "scrum cards",
                  OnClickCommand = _cardsPrintingHandler.PrintCommand,
                  Icon = new BitmapImage(new Uri(@"pack://application:,,,/Jira Agile Plugin;component/Assets/XpsIcon.png"))
               },
               new MenuEntryButton
               {
                  Label = "burndown",
                  OnClickCommand = _burndownViewModel.OpenCommand,
                  Icon = new BitmapImage(new Uri(@"pack://application:,,,/Jira Agile Plugin;component/Assets/BurndownChart.png"))
               }
            }
         };
      }

      public IEnumerable<IMicroservice> GetMicroservices()
      {
         yield return new ScrumCardsExportMicroservice();
         yield return _cardsPrintingHandler;
         yield return _burndownViewModel;
      }
   }
}
