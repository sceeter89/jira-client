using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Yakuza.JiraClient.Api;
using Yakuza.JiraClient.Api.Model;
using Yakuza.JiraClient.Service;
using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Yakuza.JiraClient.Api.Messages.Actions.Authentication;
using Yakuza.JiraClient.Api.Messages.Actions;
using Yakuza.JiraClient.Api.Messages.Status;
using Yakuza.JiraClient.Messaging.Api;

namespace Yakuza.JiraClient.ViewModel
{
   public class LoginViewModel : ViewModelBase,
      IHandleMessage<ConnectionIsBroken>,
      IHandleMessage<LoggedInMessage>,
      IHandleMessage<LoggedOutMessage>,
      IHandleMessage<IsLoggedInMessage>
   {
      private readonly Configuration _configuration;
      private readonly IMessageBus _messenger;
      private readonly IJiraOperations _operations;
      private bool _isBusy = false;
      private bool _isConnected;
      private bool _isDisconnected;
      private RawProfileDetails _profile;
      private BitmapImage _avatarSource;

      public LoginViewModel(IMessageBus messenger, Configuration configuration, IJiraOperations operations)
      {
         _messenger = messenger;
         _configuration = configuration;
         _operations = operations;

         var checkLoginTimer = new DispatcherTimer();
         checkLoginTimer.Interval = TimeSpan.FromMilliseconds(50);
         checkLoginTimer.Tick += async (s, a) =>
         {
            checkLoginTimer.IsEnabled = false;
            var sessionInfo = await _operations.CheckSession();
            if (sessionInfo.IsLoggedIn)
            {
               _configuration.IsLoggedIn = true;
               IsConnected = true;
               _messenger.LogMessage("Logged in using existing security token.");
               _messenger.Send(new LoggedInMessage());
            }
            else
            {
               _configuration.IsLoggedIn = false;
               IsConnected = false;
               _messenger.Send(new LoggedOutMessage());
            }
         };

         LoginCommand = new RelayCommand<PasswordBox>(async password => await Login(password.Password), p => _isBusy == false);
         LogoutCommand = new RelayCommand(async () => await Logout(), () => _isBusy == false);

         IsConnected = false;
         
         checkLoginTimer.IsEnabled = true;
         _messenger.Register(this);
      }
      
      private async Task Login(string password)
      {
         _isBusy = true;
         LoginCommand.RaiseCanExecuteChanged();

         try
         {
            _messenger.LogMessage("Trying to log in JIRA: " + JiraUrl);
            var result = await _operations.TryToLogin(Username, password);
            if (result.WasSuccessful)
            {
               IsConnected = true;
               _messenger.Send(new LoggedInMessage());
               _messenger.LogMessage("Logged in successfully!", LogLevel.Info);
            }
            else
            {
               IsConnected = false;
               _messenger.Send(new LoggedOutMessage());
               _messenger.LogMessage("Failed to log in! Reason: " + result.ErrorMessage, LogLevel.Warning);
            }
         }
         catch (Exception e)
         {
            _messenger.LogMessage("Stack Trace: " + Environment.NewLine + e.StackTrace, LogLevel.Debug);
            _messenger.LogMessage("Unhandled exception occured during logging in: " + e.Message, LogLevel.Critical);
         }

         _isBusy = false;
         LoginCommand.RaiseCanExecuteChanged();
      }

      private async Task Logout()
      {
         _isBusy = true;
         LogoutCommand.RaiseCanExecuteChanged();

         try
         {
            _messenger.LogMessage("Logging out...");
            await _operations.Logout();
            IsConnected = false;
            _messenger.Send(new LoggedOutMessage());
            _messenger.LogMessage("Logged out successfully", LogLevel.Info);

         }
         catch (Exception e)
         {
            _messenger.LogMessage("Stack Trace: " + Environment.NewLine + e.StackTrace, LogLevel.Debug);
            _messenger.LogMessage("Unhandled exception occured during logging out: " + e.Message, LogLevel.Critical);
         }

         _isBusy = false;
         LogoutCommand.RaiseCanExecuteChanged();
      }

      public void Handle(ConnectionIsBroken message)
      {
         _messenger.LogMessage("Connection is broken. Security token might have been invalidated.");
         if (IsConnected)
         {
            _messenger.Send(new LoggedOutMessage());
         }
         IsConnected = false;
      }

      public async void Handle(LoggedInMessage message)
      {
         var details = await _operations.GetProfileDetails();
         Profile = details;
         var avatar = _operations.DownloadPicture(details.AvatarUrls.Avatar48x48);
         AvatarSource = avatar;
         IsConnected = true;
      }

      public void Handle(LoggedOutMessage message)
      {
         Profile = null;
         AvatarSource = null;
         IsConnected = false;
      }

      public void Handle(IsLoggedInMessage message)
      {
         if (IsConnected)
            _messenger.Send(new ConnectionEstablishedMessage(Profile));
         else
            _messenger.Send(new ConnectionDownMessage());
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

      public bool IsConnected
      {
         get
         {
            return _isConnected;
         }
         set
         {
            _isConnected = value;
            _isDisconnected = !value;

            RaisePropertyChanged();
            RaisePropertyChanged(() => IsDisconnected);
         }
      }

      public bool IsDisconnected
      {
         get
         {
            return _isDisconnected;
         }
         set
         {
            _isDisconnected = value;
            _isConnected = !value;

            RaisePropertyChanged();
            RaisePropertyChanged(() => IsConnected);
         }
      }

      public RelayCommand<PasswordBox> LoginCommand { get; private set; }
      public RelayCommand LogoutCommand { get; private set; }

      public RawProfileDetails Profile
      {
         get { return _profile; }
         set
         {
            _profile = value;
            RaisePropertyChanged();

         }
      }

      public BitmapImage AvatarSource
      {
         get { return _avatarSource; }
         set
         {
            _avatarSource = value;
            RaisePropertyChanged();
         }
      }
   }
}