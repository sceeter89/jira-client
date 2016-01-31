using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using JiraAssistant.Model;
using JiraAssistant.Model.Exceptions;
using JiraAssistant.Pages;
using JiraAssistant.Services;
using JiraAssistant.Services.Resources;
using System;
using System.Windows.Controls;

namespace JiraAssistant.ViewModel
{
   public class LoginPageViewModel : ViewModelBase
   {
      private readonly JiraSessionViewModel _jiraSession;
      private readonly JiraSessionService _sessionService;
      private readonly INavigator _navigator;
      private string _loginErrorMessage;
      private string _jiraAddress;
      private string _username;
      private bool _isBusy;

      public LoginPageViewModel(INavigator navigator,
         JiraSessionViewModel jiraSession,
         JiraSessionService sessionService,
         AssistantConfiguration configuration)
      {
         _navigator = navigator;
         _jiraSession = jiraSession;
         _sessionService = sessionService;

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

      private async void Login(PasswordBox passwordBox)
      {
         try
         {
            IsBusy = true;

            await _sessionService.AttemptLogin(JiraAddress, Username, passwordBox.Password);

            _jiraSession.LoggedIn();
            _navigator.NavigateTo(new MainMenuPage());
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
            IsBusy = true;

            if (await _sessionService.CheckJiraSession())
            {
               _jiraSession.LoggedIn();
               _navigator.NavigateTo(new MainMenuPage());
            }
         }
         finally
         {
            IsBusy = false;
         }
      }
   }
}
