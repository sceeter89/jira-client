using Autofac;
using System.Reflection;
using Yakuza.JiraClient.Messaging;
using Yakuza.JiraClient.Api;
using Yakuza.JiraClient.Messaging.Api;
using System;
using Yakuza.JiraClient.Api.Plugins;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using GalaSoft.MvvmLight;
using Yakuza.JiraClient.Api.Messages.IO.Plugins;
using System.Threading.Tasks;

namespace Yakuza.JiraClient.ViewModel
{
   public class ViewModelLocator
   {
      public ViewModelLocator()
      {
         if (ViewModelBase.IsInDesignModeStatic)
            return;

         BuildIocContainer();
         Task.Factory.StartNew(async () =>
         {
            await Task.Delay(TimeSpan.FromMilliseconds(100));
            LoadPlugins();
         });
      }

      public MainViewModel Main
      {
         get
         {
            return IocContainer.Resolve<MainViewModel>();
         }
      }

      public LogViewModel Logging
      {
         get
         {
            return IocContainer.Resolve<LogViewModel>();
         }
      }

      public ConnectionViewModel Connection
      {
         get
         {
            return IocContainer.Resolve<ConnectionViewModel>();
         }
      }

      public SearchIssuesViewModel Search
      {
         get
         {
            return IocContainer.Resolve<SearchIssuesViewModel>();
         }
      }

      public IssueListViewModel IssueList
      {
         get
         {
            return IocContainer.Resolve<IssueListViewModel>();
         }
      }

      public MenuBarViewModel MenuBar
      {
         get
         {
            return IocContainer.Resolve<MenuBarViewModel>();
         }
      }

      public static void Cleanup()
      {
         // TODO Clear the ViewModels
      }

      internal IContainer IocContainer { get; private set; }

      private void BuildIocContainer()
      {
         var builder = new ContainerBuilder();

         var clientAssembly = Assembly.Load("Jira Client");
         var ioAssembly = Assembly.Load("Yakuza.JiraClient.IO");

         var messageBus = new MessageBus();

         builder.Register(x => messageBus)
            .As<IMessageBus>()
            .SingleInstance();

         //Register ViewModels
         builder.RegisterAssemblyTypes(clientAssembly)
            .Where(t => t.Name.EndsWith("ViewModel"))
            .AsSelf()
            .SingleInstance();

         //Register and run background services
         builder.RegisterAssemblyTypes(clientAssembly)
            .InNamespace("Yakuza.JiraClient.Service")
            .AsImplementedInterfaces()
            .AsSelf()
            .SingleInstance()
            .AutoActivate();
         builder.RegisterAssemblyTypes(ioAssembly)
            .AssignableTo<IMicroservice>()
            .AsSelf()
            .SingleInstance()
            .AutoActivate()
            .OnActivated(s => (s.Instance as IMicroservice).Initialize(messageBus));

         IocContainer = builder.Build();
      }

      [ImportMany]
      private IEnumerable<Lazy<IJiraClientPlugin>> _pluginDefinitions;
      private readonly IList<IMicroservice> _loadedMicroservices = new List<IMicroservice>();
      private CompositionContainer _container;

      private void LoadPlugins()
      {
         var catalog = new AggregateCatalog();
         catalog.Catalogs.Add(new DirectoryCatalog(Environment.CurrentDirectory));

         var pluginsSubdir = Path.Combine(Environment.CurrentDirectory, "Extensions");
         if (Directory.Exists(pluginsSubdir))
            catalog.Catalogs.Add(new DirectoryCatalog(pluginsSubdir));

         _container = new CompositionContainer(catalog);
         _container.ComposeParts(this);

         var messageBus = IocContainer.Resolve<IMessageBus>();

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
               microservice.Initialize(messageBus);
               _loadedMicroservices.Add(microservice);
            }
         }

         foreach (var pluginReference in _pluginDefinitions)
         {
            var plugin = pluginReference.Value;
            if (string.IsNullOrWhiteSpace(plugin.PluginName))
               continue;

            messageBus.Send(new NewPluginFoundMessage(plugin));
         }
      }
   }
}