using System.Collections.Generic;

namespace Yakuza.JiraClient.Api.Plugins
{
   /// <summary>
   /// If you create a plugin make sure you implement this interface with metadata about your plugin.
   /// Feel free to return nulls if you do not plan to use it.
   /// 
   /// Please return some meaningful plugin name. If will be null or empty then plugin will be skipped.
   /// </summary>
   public interface IJiraClientPlugin
   {
      string PluginName { get; }
      IEnumerable<IMicroservice> Microservices { get; }
      MenuEntryDescriptor MenuEntryDescriptor { get; }
   }
}
