using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using JiraManager.Api;
using JiraManager.Service;
using Microsoft.Practices.ServiceLocation;

namespace JiraManager.ViewModel
{
   public class ViewModelLocator
   {
      public ViewModelLocator()
      {
         ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

         if (ViewModelBase.IsInDesignModeStatic)
         {
            // Create design time view services and models
         }
         else
         {
            // Create run time view services and models
            SimpleIoc.Default.Register(() => Messenger.Default);
         }

         SimpleIoc.Default.Register<IJiraOperations, JiraOperations>();

         SimpleIoc.Default.Register<Configuration>(true);
         SimpleIoc.Default.Register<IssuesRetriever>(true);
         SimpleIoc.Default.Register<ConnectionChecker>(true);

         SimpleIoc.Default.Register<LoginViewModel>();
         SimpleIoc.Default.Register<LogViewModel>();
         SimpleIoc.Default.Register<SearchIssuesViewModel>();
         SimpleIoc.Default.Register<IssueListViewModel>();
         SimpleIoc.Default.Register<MainViewModel>();
      }

      public MainViewModel Main
      {
         get
         {
            return ServiceLocator.Current.GetInstance<MainViewModel>();
         }
      }

      public LogViewModel Logging
      {
         get
         {
            return ServiceLocator.Current.GetInstance<LogViewModel>();
         }
      }

      public LoginViewModel Login
      {
         get
         {
            return ServiceLocator.Current.GetInstance<LoginViewModel>();
         }
      }

      public SearchIssuesViewModel Search
      {
         get
         {
            return ServiceLocator.Current.GetInstance<SearchIssuesViewModel>();
         }
      }

      public IssueListViewModel IssueList
      {
         get
         {
            return ServiceLocator.Current.GetInstance<IssueListViewModel>();
         }
      }

      public static void Cleanup()
      {
         // TODO Clear the ViewModels
      }
   }
}