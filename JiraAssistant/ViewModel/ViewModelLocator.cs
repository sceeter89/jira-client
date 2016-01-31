using Autofac;

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
            .AsSelf()
            .AsImplementedInterfaces()
            .SingleInstance();

         IocContainer = builder.Build();
      }
   }
}