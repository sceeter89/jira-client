using LightShell.Api;
using LightShell.Api.Plugins;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Media.Imaging;

namespace Yakuza.JiraClient.Plugins.Agile
{
   [Export(typeof(ILightShellPlugin))]
   public class AgilePlugin : ILightShellPlugin
   {
      private readonly CardsPrintingHandler _cardsPrintingHandler = new CardsPrintingHandler();

      public string PluginName
      {
         get
         {
            return "Agile plugin";
         }
      }

      public IEnumerable<MenuEntryDescriptor> GetMenuEntries()
      {
         yield return new MenuEntryDescriptor
         {
            ButtonsGroupName = "agile",
            Tab = MenuTab.Home,
            Buttons = new[]
            {
               new MenuEntryButton
               {
                  Label = "scrum cards",
                  OnClickCommand = _cardsPrintingHandler.PrintCommand,
                  Icon = new BitmapImage(new Uri(@"pack://application:,,,/Jira Agile Plugin;component/Assets/XpsIcon.png"))
               }
            }
         };
      }

      public IEnumerable<IMicroservice> GetMicroservices()
      {
         yield return new ScrumCardsExportMicroservice();
         yield return _cardsPrintingHandler;
      }
   }
}
