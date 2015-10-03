using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Yakuza.JiraClient.Api;
using Yakuza.JiraClient.Api.Plugins;

namespace Yakuza.JiraClient.Plugins.Agile
{
   [Export(typeof(IJiraClientPlugin))]
   public class AgilePlugin : IJiraClientPlugin
   {
      public string PluginName
      {
         get
         {
            return "Agile plugin";
         }
      }

      public IEnumerable<MenuEntryDescriptor> GetMenuEntries()
      {
         return null;
      }

      public IEnumerable<IMicroservice> GetMicroservices()
      {
         yield return new ScrumCardsExportMicroservice();
      }
   }
}
