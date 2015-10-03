using Yakuza.JiraClient.Api.Plugins;
using Yakuza.JiraClient.Messaging.Api;

namespace Yakuza.JiraClient.Api.Messages.IO.Plugins
{
   public class NewPluginFoundMessage : IMessage
   {
      public NewPluginFoundMessage(IJiraClientPlugin pluginDescription)
      {
         PluginDescription = pluginDescription;
      }

      public IJiraClientPlugin PluginDescription { get; private set; }
   }
}
