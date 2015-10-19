using System.Collections.Generic;

namespace LightShell.Api.Plugins
{
   /// <summary>
   /// If you create a plugin make sure you implement this interface with metadata about your plugin.
   /// If you are not going to use some of functionalities then either return empty IEnumerable or type "yield break;".
   /// 
   /// Please return some meaningful plugin name. If will be empty then plugin will be skipped.
   /// </summary>
   public interface ILightShellPlugin
   {
      string PluginName { get; }
      IEnumerable<IMicroservice> GetMicroservices();
      IEnumerable<MenuEntryDescriptor> GetMenuEntries();
   }
}
