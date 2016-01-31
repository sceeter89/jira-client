using Autofac;
using JiraAssistant.Model;
using JiraAssistant.Services.Resources;

namespace JiraAssistant.ViewModel
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
         get
         {
            return IocContainer.Resolve<MainViewModel>();
         }
      }

      public LoginPageViewModel Login
      {
         get
         {
            return IocContainer.Resolve<LoginPageViewModel>();
         }
      }

      public JiraSessionViewModel JiraSession
      {
         get
         {
            return IocContainer.Resolve<JiraSessionViewModel>();
         }
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

         builder.RegisterAssemblyTypes(GetType().Assembly)
            .InNamespaceOf<ViewModelLocator>()
            .Except<ViewModelLocator>()
            .AsSelf()
            .AsImplementedInterfaces()
            .SingleInstance();

         builder.RegisterAssemblyTypes(GetType().Assembly)
            .InNamespaceOf<BaseRestService>()
            .Except<BaseRestService>()
            .AsSelf()
            .AsImplementedInterfaces()
            .SingleInstance();

         builder.RegisterType<AssistantConfiguration>()
            .AsSelf()
            .SingleInstance();

         IocContainer = builder.Build();
      }
   }
}