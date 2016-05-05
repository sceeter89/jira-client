using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using JiraAssistant.Model.Exceptions;
using JiraAssistant.Pages;
using JiraAssistant.Services;
using JiraAssistant.Services.Jira;
using JiraAssistant.Settings;
using System;
using System.Windows.Controls;

namespace JiraAssistant.ViewModel
{
   public class LoginPageViewModel : ViewModelBase
   {
      private readonly JiraSessionViewModel _jiraSession;
      private readonly INavigator _navigator;
      private string _loginErrorMessage;
      private string _jiraAddress;
      private string _username;
      private bool _isBusy;
      private string _busyMessage;
      private readonly IJiraApi _jiraApi;

      public LoginPageViewModel(INavigator navigator,
         JiraSessionViewModel jiraSession,
         IJiraApi jiraApi,
         AssistantSettings configuration)
      {
         _navigator = navigator;
         _jiraSession = jiraSession;
         _jiraApi = jiraApi;

         JiraAddress = configuration.JiraUrl;
         Username = configuration.Username;
      }

      public RelayCommand<PasswordBox> LoginCommand { get { return new RelayCommand<PasswordBox>(Login); } }

      public string LoginErrorMessage
      {
         get { return _loginErrorMessage; }
         set
         {
            _loginErrorMessage = value;
            RaisePropertyChanged();
         }
      }

      public string JiraAddress
      {
         get { return _jiraAddress; }
         set
         {
            _jiraAddress = value;
            RaisePropertyChanged();
         }
      }

      public string Username
      {
         get { return _username; }
         set
         {
            _username = value;
            RaisePropertyChanged();
         }
      }

      public bool IsBusy
      {
         get { return _isBusy; }
         set
         {
            _isBusy = value;
            RaisePropertyChanged();
         }
      }

      public string BusyMessage
      {
         get { return _busyMessage; }
         set
         {
            _busyMessage = value;
            RaisePropertyChanged();
         }
      }

      private async void Login(PasswordBox passwordBox)
      {
         try
         {
            BusyMessage = "Trying to log into JIRA...";
            IsBusy = true;
            await _jiraApi.Session.AttemptLogin(JiraAddress, Username, passwordBox.Password);
            
            _navigator.NavigateTo(new PickUpAgileBoardPage());
         }
         catch (LoginFailedException e)
         {
            LoginErrorMessage = e.Message;
         }
         catch (Exception)
         {
            LoginErrorMessage = "Failed to log in due to technical issues.";
         }
         finally
         {
            passwordBox.Password = string.Empty;
            IsBusy = false;
         }
      }

      internal async void AttemptAutoLogin()
      {
         try
         {
            BusyMessage = "Checking existing credentials...";
            IsBusy = true;

            if (await _jiraApi.Session.CheckJiraSession())
            {
               _navigator.NavigateTo(new PickUpAgileBoardPage());
               _jiraSession.LoggedIn();
            }
         }
         catch(IncompleteJiraConfiguration) { }
         finally
         {
            IsBusy = false;
         }
      }
   }
}
