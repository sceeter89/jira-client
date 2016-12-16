using System;
using System.Reflection;
using Autofac;
using GalaSoft.MvvmLight.Messaging;
using Gtk;
using JiraAssistant.Logic.ContextlessViewModels;
using JiraAssistant.Logic.Services;
using JiraAssistant.Logic.Settings;

namespace JiraAssistant.Mono
{
	internal class Bootstrap
	{
		public static void Main(string[] args)
		{
			AppDomain.CurrentDomain.DomainUnload += (sender, e) =>
			{
				IocContainer.Dispose();
			};

			InitializeIoc();

			Application.Init();
			MainWindow = IocContainer.Resolve<MainWindow>();
			MainWindow.Show();

			AttemptToLogin();

			Application.Run();
		}

		public static IContainer IocContainer { get; private set; }
		public static MainWindow MainWindow { get; private set; }

		private static void InitializeIoc()
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

			builder.RegisterAssemblyTypes(typeof(Bootstrap).Assembly)
				   .InNamespace("JiraAssistant.Mono.Controllers")
				   .AsSelf()
				   .SingleInstance();

			builder.RegisterType<JiraApi>()
			   .SingleInstance()
			   .AsImplementedInterfaces()
			   .AutoActivate();

			builder.RegisterType<NavigationService>()
			   .SingleInstance()
			   .AutoActivate();

			builder.RegisterType<DesktopNotificationsService>()
			   .SingleInstance()
			   .AutoActivate();

			builder.RegisterType<Messenger>()
			   .As<IMessenger>()
			   .SingleInstance();

			builder.RegisterType<MainWindow>()
			   .AsSelf()
			   .SingleInstance();

			builder.RegisterType<OnUiThread>()
				   .AsImplementedInterfaces()
				   .SingleInstance();

			IocContainer = builder.Build();
		}

		private static void AttemptToLogin()
		{
			IocContainer.Resolve<LoginPageViewModel>().AttemptAutoLogin();
		}
	}
}
