using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LightShell.Api;
using LightShell.Api.Model;
using LightShell.Service;
using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using LightShell.Api.Messages.Actions.Authentication;
using LightShell.Api.Messages.Actions;
using LightShell.Api.Messages.Status;
using LightShell.Messaging.Api;
using LightShell.Api.Messages.IO.Jira;
using LightShell.InternalMessages.UI;

namespace LightShell.ViewModel
{
   internal class ConnectionViewModel : ViewModelBase,
      ICoreViewModel,
      IHandleMessage<ConnectionIsBroken>,
      IHandleMessage<LoggedInMessage>,
      IHandleMessage<LoggedOutMessage>,
      IHandleMessage<IsLoggedInMessage>,
      IHandleMessage<GetProfileDetailsResponse>,
      IHandleMessage<DownloadPictureResponse>,
      IHandleMessage<AttemptLoginResponse>,
      IHandleMessage<CheckJiraSessionResponse>
   {
      private readonly Configuration _configuration;
      private readonly IMessageBus _messageBus;
      private bool _isBusy = false;
      private bool _isConnected;
      private bool _isDisconnected;
      private RawProfileDetails _profile;
      private BitmapImage _avatarSource;

      public ConnectionViewModel(IMessageBus messageBus, Configuration configuration)
      {
         _messageBus = messageBus;
         _configuration = configuration;

         var checkLoginTimer = new DispatcherTimer();
         checkLoginTimer.Interval = TimeSpan.FromMilliseconds(50);
         checkLoginTimer.Tick += (s, a) =>
         {
            _messageBus.Send(new CheckJiraSessionMessage());
            checkLoginTimer.IsEnabled = false;
         };

         IsConnected = false;

         checkLoginTimer.IsEnabled = true;
         _messageBus.Register(this);
      }

      private void SetIsBusy(bool isBusy)
      {
         _isBusy = isBusy;
         LogoutCommand.RaiseCanExecuteChanged();
         LoginCommand.RaiseCanExecuteChanged();
      }

      private void Login(string password)
      {
         SetIsBusy(true);
         _messageBus.LogMessage("Trying to log in JIRA: " + JiraUrl);
         _messageBus.Send(new AttemptLoginMessage(JiraUrl, Username, password));
      }

      private void Logout()
      {
         SetIsBusy(true);
         _messageBus.LogMessage("Logging out...");
         _messageBus.Send(new LogoutMessage());
      }

      public void Handle(ConnectionIsBroken message)
      {
         _messageBus.LogMessage("Connection is broken. Security token might have been invalidated.");
         if (IsConnected)
         {
            _messageBus.Send(new LoggedOutMessage());
         }
         IsConnected = false;
      }

      public void Handle(LoggedInMessage message)
      {
         _messageBus.Send(new GetProfileDetailsMessage());
         IsConnected = true;

         SetIsBusy(false);
      }

      public void Handle(LoggedOutMessage message)
      {
         Profile = null;
         AvatarSource = null;
         IsConnected = false;
         _messageBus.LogMessage("Logged out successfully", LogLevel.Info);

         SetIsBusy(false);
      }

      public void Handle(IsLoggedInMessage message)
      {
         if (IsConnected)
            _messageBus.Send(new ConnectionEstablishedMessage(Profile));
         else
            _messageBus.Send(new ConnectionDownMessage());
      }

      public void Handle(GetProfileDetailsResponse message)
      {
         Profile = message.Details;
         _messageBus.Send(new DownloadPictureMessage(Profile.AvatarUrls.Avatar48x48));
      }

      public void Handle(DownloadPictureResponse message)
      {
         AvatarSource = message.Image;
      }

      public void Handle(AttemptLoginResponse message)
      {
         if (message.Result.WasSuccessful)
         {
            IsConnected = true;
            _messageBus.Send(new LoggedInMessage());
            _messageBus.LogMessage("Logged in successfully!", LogLevel.Info);
         }
         else
         {
            IsConnected = false;
            _messageBus.Send(new LoggedOutMessage());
            _messageBus.LogMessage("Failed to log in! Reason: " + message.Result.ErrorMessage, LogLevel.Warning);
         }

         _isBusy = false;
         LogoutCommand.RaiseCanExecuteChanged();
         LoginCommand.RaiseCanExecuteChanged();
      }

      public void Handle(CheckJiraSessionResponse message)
      {
         if (IsConnected == false && message.Response.IsLoggedIn)
         {
            IsConnected = true;
            _messageBus.LogMessage("Logged in using existing security token.");
            _messageBus.Send(new LoggedInMessage());
         }
         else if (IsConnected && message.Response.IsLoggedIn == false)
         {
            IsConnected = false;
            _messageBus.Send(new LoggedOutMessage());
         }
      }

      public void OnControlInitialized()
      {
         _messageBus.Send(new ViewModelInitializedMessage(this.GetType()));
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

      private RelayCommand<PasswordBox> _loginCommand;
      public RelayCommand<PasswordBox> LoginCommand
      {
         get
         {
            if (_loginCommand == null)
               _loginCommand = new RelayCommand<PasswordBox>(password => Login(password.Password), p => _isBusy == false);

            return _loginCommand;
         }
      }

      private RelayCommand _logoutCommand;
      public RelayCommand LogoutCommand
      {
         get
         {
            if(_logoutCommand == null)
               _logoutCommand = new RelayCommand(() => Logout(), () => _isBusy == false);

            return _logoutCommand;
         }
      }

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