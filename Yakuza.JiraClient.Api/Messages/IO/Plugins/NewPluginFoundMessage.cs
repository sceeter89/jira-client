using System.Reflection;
using Yakuza.JiraClient.Api.Plugins;
using Yakuza.JiraClient.Messaging.Api;

namespace Yakuza.JiraClient.Api.Messages.IO.Plugins
{
   public class NewPluginFoundMessage : IMessage
   {
      public NewPluginFoundMessage(IJiraClientPlugin pluginDescription, Assembly sourceAssembly)
      {
         PluginDescription = pluginDescription;
         SourceAssembly = sourceAssembly;
      }

      public IJiraClientPlugin PluginDescription { get; private set; }
      public Assembly SourceAssembly { get; private set; }
   }
}
