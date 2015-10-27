using LightShell.Api.Plugins;
using System.ComponentModel.Composition;
using LightShell.Api;
using System.Collections.Generic;
using LightShell.Plugin.Jira.Microservices;
using LightShell.Plugin.Jira.Api;
using LightShell.Service;
using LightShell.Plugin.Jira.Properties;

namespace LightShell.Plugin.Jira
{
   [Export(typeof(ILightShellPlugin))]
   public class JiraPlugin : ILightShellPlugin
   {
      static JiraPlugin()
      {
         if (Settings.Default.SettingsUpgradePending)
         {
            Settings.Default.Upgrade();
            Settings.Default.SettingsUpgradePending = false;
            Settings.Default.Save();
         }
      }
      private readonly IConfiguration _configuration = new Configuration();

      public string PluginName
      {
         get { return "JIRA Plugin"; }
      }

      public IEnumerable<MenuEntryDescriptor> GetMenuEntries()
      {
         yield break;
      }

      public IEnumerable<IMicroservice> GetMicroservices()
      {
         yield return new ConnectionChecker();
         yield return new IssuesSearchMicroservice(_configuration);
         yield return new MetadataRetriever(_configuration);
         yield return new ResourceDownloaderMicroservice(_configuration);
         yield return new SessionInteractionMicroservice(_configuration);
         yield return new UiManager(_configuration);
         yield return new JiraAgileIntegrationMicroservice(_configuration);
      }
   }
}
