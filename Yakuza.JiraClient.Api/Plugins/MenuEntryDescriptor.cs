using System.Collections.Generic;

namespace Yakuza.JiraClient.Api.Plugins
{
   public class MenuEntryDescriptor
   {
      public MenuTab Tab { get; set; }

      public string ButtonsGroupName { get; set; }

      public IEnumerable<MenuEntryButton> Buttons { get; set; }
   }
}