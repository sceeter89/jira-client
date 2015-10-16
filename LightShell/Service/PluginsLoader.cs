using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Threading.Tasks;
using LightShell.Api;
using LightShell.Api.Messages.IO.Plugins;
using LightShell.Api.Plugins;
using LightShell.InternalMessages.UI;
using LightShell.Messaging.Api;

namespace LightShell.Service
{
   internal class PluginsLoader :
      IHandleMessage<CoreUserInterfaceLoadedMessage>
   {
      public PluginsLoader(IMessageBus messageBus)
      {
         _messageBus = messageBus;

         _messageBus.Register(this);
      }

      [ImportMany]
      private IEnumerable<Lazy<ILightShellPlugin>> _pluginDefinitions;
      private readonly IList<IMicroservice> _loadedMicroservices = new List<IMicroservice>();
      private CompositionContainer _container;
      private readonly IMessageBus _messageBus;

      private void LoadPlugins()
      {
         var catalog = new AggregateCatalog();
         catalog.Catalogs.Add(new DirectoryCatalog(Environment.CurrentDirectory));

         var pluginsSubdir = Path.Combine(Environment.CurrentDirectory, "Extensions");
         if (Directory.Exists(pluginsSubdir))
            catalog.Catalogs.Add(new DirectoryCatalog(pluginsSubdir));

         _container = new CompositionContainer(catalog);
         _container.ComposeParts(this);

         foreach (var pluginReference in _pluginDefinitions)
         {
            var plugin = pluginReference.Value;
            if (string.IsNullOrWhiteSpace(plugin.PluginName))
               continue;

            var exportedMicroservices = plugin.GetMicroservices();
            if (exportedMicroservices == null)
               continue;
            foreach (var microservice in exportedMicroservices)
            {
               microservice.Initialize(_messageBus);
               _loadedMicroservices.Add(microservice);
            }
         }

         foreach (var pluginReference in _pluginDefinitions)
         {
            var plugin = pluginReference.Value;
            if (string.IsNullOrWhiteSpace(plugin.PluginName))
               continue;

            _messageBus.Send(new NewPluginFoundMessage(plugin));
         }

         _messageBus.Send(new PluginsLoadedMessage());
      }

      public void Handle(CoreUserInterfaceLoadedMessage message)
      {
         Task.Factory.StartNew(() => LoadPlugins());
      }
   }
}
