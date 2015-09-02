using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using JiraManager.Api;
using JiraManager.Messages.Actions.Authentication;
using JiraManager.Service;
using System;
using System.Windows.Threading;

namespace JiraManager.ViewModel
{
   public class LoginViewModel : ViewModelBase
   {
      private readonly Configuration _configuration;
      private readonly IMessenger _messenger;
      private readonly IJiraOperations _operations;
      private string _password;
      private bool _isBusy = false;

      public LoginViewModel(IMessenger messenger, Configuration configuration, IJiraOperations operations)
      {
         _messenger = messenger;
         _configuration = configuration;
         _operations = operations;

         var checkLoginTimer = new DispatcherTimer();
         checkLoginTimer.Interval = TimeSpan.FromMilliseconds(100);
         checkLoginTimer.Tick += async (s, a) =>
         {
            var sessionInfo = await _operations.CheckSession();
            if (sessionInfo.IsLoggedIn)
            {
               _messenger.Send(new LoggedInMessage());
               _configuration.IsLoggedIn = true;
            }
            else
            {
               _messenger.Send(new LoggedOutMessage());
               _configuration.IsLoggedIn = false;
            }
            checkLoginTimer.IsEnabled = false;
         };
         checkLoginTimer.IsEnabled = true;
         LoginCommand = new RelayCommand(async () =>
         {
            _isBusy = true;
            LoginCommand.RaiseCanExecuteChanged();



            _isBusy = false;
            LoginCommand.RaiseCanExecuteChanged();
         }, ()=>_isBusy == false);
      }

      public string JiraUrl
      {
         get { return _configuration.JiraUrl; }
         set
         {
            _configuration.JiraUrl = value;
            RaisePropertyChanged();
         }
      }

      public string Username
      {
         get { return _configuration.Username; }
         set
         {
            _configuration.Username = value;
            RaisePropertyChanged();
         }
      }

      public string Password
      {
         get { return _password; }
         set
         {
            _password = value;
            RaisePropertyChanged();
         }
      }

      public RelayCommand LoginCommand {get; private set;}
      public RelayCommand LogoutCommand {get; private set;}
   }
}