using Autofac;
using GalaSoft.MvvmLight.Messaging;
using JiraAssistant.Pages;
using JiraAssistant.Logic.Services;
using JiraAssistant.Logic.Settings;
using System.Reflection;
using JiraAssistant.Logic.ContextlessViewModels;

namespace JiraAssistant
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            BuildContainer();
        }

        public static IContainer IocContainer { get; set; }

        public MainViewModel Main
        {
            get { return IocContainer.Resolve<MainViewModel>(); }
        }

        public LoginPageViewModel Login
        {
            get { return IocContainer.Resolve<LoginPageViewModel>(); }
        }

        public JiraSessionViewModel JiraSession
        {
            get { return IocContainer.Resolve<JiraSessionViewModel>(); }
        }

        public AgileBoardSelectViewModel AgileBoardSelect
        {
            get { return IocContainer.Resolve<AgileBoardSelectViewModel>(); }
        }

        public GraveyardSettings GraveyardSettings
        {
            get { return IocContainer.Resolve<GraveyardSettings>(); }
        }

        public GeneralSettings GeneralSettings
        {
            get { return IocContainer.Resolve<GeneralSettings>(); }
        }

        public IssuesSettings IssuesSettings
        {
            get { return IocContainer.Resolve<IssuesSettings>(); }
        }

        public AnalysisSettings AnalysisSettings
        {
            get { return IocContainer.Resolve<AnalysisSettings>(); }
        }

        public ReportsSettings ReportsSettings
        {
            get { return IocContainer.Resolve<ReportsSettings>(); }
        }

        public RecentUpdatesViewModel RecentUpdates
        {
            get { return IocContainer.Resolve<RecentUpdatesViewModel>(); }
        }

        public static void Cleanup()
        {
            IocContainer.Dispose();
        }

        private void BuildContainer()
        {
            if (IocContainer != null)
                return;

            var builder = new ContainerBuilder();

            var logicAssembly = Assembly.Load("JiraAssistant.Logic");

            builder.RegisterAssemblyTypes(logicAssembly)
               .InNamespace("JiraAssistant.Logic.ViewModels")
               .AsSelf()
               .AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(logicAssembly)
               .InNamespace("JiraAssistant.Logic.ContextlessViewModels")
               .AsSelf()
               .AsImplementedInterfaces()
               .SingleInstance();

            builder.RegisterAssemblyTypes(logicAssembly)
               .InNamespace("JiraAssistant.Logic.Settings")
               .Except<SettingsBase>()
               .AsSelf()
               .SingleInstance();

            builder.RegisterAssemblyTypes(GetType().Assembly)
               .InNamespaceOf<BaseNavigationPage>()
               .Except<BaseNavigationPage>()
               .AsSelf()
               .SingleInstance();

            builder.RegisterAssemblyTypes(logicAssembly)
               .InNamespace("JiraAssistant.Logic.Services")
               .AsImplementedInterfaces()
               .AsSelf()
               .SingleInstance();

            builder.RegisterAssemblyTypes(logicAssembly)
               .InNamespace("JiraAssistant.Logic.Services.Daemons")
               .AsImplementedInterfaces()
               .AsSelf()
               .SingleInstance()
               .AutoActivate();

            builder.RegisterType<JiraApi>()
               .SingleInstance()
               .AsImplementedInterfaces()
               .AutoActivate();

            builder.RegisterType<NavigationService>()
               .SingleInstance()
               .AsImplementedInterfaces()
               .AutoActivate();

            builder.RegisterType<Messenger>()
               .As<IMessenger>()
               .SingleInstance();

            IocContainer = builder.Build();
        }
    }
}