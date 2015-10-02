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

namespace Yakuza.JiraClient.ViewModel
{
   public class ViewModelLocator
   {
      public ViewModelLocator()
      {
         if (ViewModelBase.IsInDesignModeStatic)
            return;

         BuildIocContainer();
         LoadPlugins();
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

      public PivotGridViewModel Pivot
      {
         get
         {
            return IocContainer.Resolve<PivotGridViewModel>();
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
      private CompositionContainer _container;

      private void LoadPlugins()
      {
         var catalog = new AggregateCatalog();
         catalog.Catalogs.Add(new DirectoryCatalog(Environment.CurrentDirectory));
         catalog.Catalogs.Add(new DirectoryCatalog(Path.Combine(Environment.CurrentDirectory, "Plugins")));
         _container = new CompositionContainer(catalog);
         _container.ComposeParts(this);


      }
   }
}