using LightShell.Api.Plugins;
using LightShell.Messaging.Api;

namespace LightShell.Api.Messages.IO.Plugins
{
   public class NewPluginFoundMessage : IMessage
   {
      public NewPluginFoundMessage(ILightShellPlugin pluginDescription)
      {
         PluginDescription = pluginDescription;
      }

      public ILightShellPlugin PluginDescription { get; private set; }
   }
}
