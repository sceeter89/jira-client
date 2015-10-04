using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Media.Imaging;
using Yakuza.JiraClient.Api;
using Yakuza.JiraClient.Api.Messages.Actions;
using Yakuza.JiraClient.Api.Plugins;

namespace Yakuza.JiraClient.Plugins.Agile
{
   [Export(typeof(IJiraClientPlugin))]
   public class AgilePlugin : IJiraClientPlugin
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
            Buttons = new []
            {
               new MenuEntryButton
               {
                  Label = "scrum cards",
                  OnClick = _ => _cardsPrintingHandler.PrintCards(),
                  Icon = new BitmapImage(new Uri(@"pack://application:,,,/JiraClient Agile Plugin;component/Assets/XpsIcon.png"))
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
