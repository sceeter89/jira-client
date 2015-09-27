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
using Yakuza.JiraClient.Api.Messages.IO.Jira;

namespace Yakuza.JiraClient.ViewModel
{
   public class LoginViewModel : ViewModelBase,
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
      private readonly IMessageBus _messenger;
      private bool _isBusy = false;
      private bool _isConnected;
      private bool _isDisconnected;
      private RawProfileDetails _profile;
      private BitmapImage _avatarSource;

      public LoginViewModel(IMessageBus messenger, Configuration configuration)
      {
         _messenger = messenger;
         _configuration = configuration;

         var checkLoginTimer = new DispatcherTimer();
         checkLoginTimer.Interval = TimeSpan.FromMilliseconds(50);
         checkLoginTimer.Tick += (s, a) =>
         {
            _messenger.Send(new CheckJiraSessionMessage());
            checkLoginTimer.IsEnabled = false;
         };

         IsConnected = false;

         checkLoginTimer.IsEnabled = true;
         _messenger.Register(this);
      }

      private async Task Login(string password)
      {
         _isBusy = true;
         LogoutCommand.RaiseCanExecuteChanged();
         LoginCommand.RaiseCanExecuteChanged();
         _messenger.LogMessage("Trying to log in JIRA: " + JiraUrl);
         _messenger.Send(new AttemptLoginMessage(JiraUrl, Username, password));
      }

      private async Task Logout()
      {
         _isBusy = true;
         LogoutCommand.RaiseCanExecuteChanged();
         LoginCommand.RaiseCanExecuteChanged();

         _messenger.LogMessage("Logging out...");
         _messenger.Send(new LogoutMessage());
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

      public void Handle(LoggedInMessage message)
      {
         _messenger.Send(new GetProfileDetailsMessage());
         IsConnected = true;

         _isBusy = false;
         LogoutCommand.RaiseCanExecuteChanged();
         LoginCommand.RaiseCanExecuteChanged();
      }

      public void Handle(LoggedOutMessage message)
      {
         Profile = null;
         AvatarSource = null;
         IsConnected = false;
         _messenger.LogMessage("Logged out successfully", LogLevel.Info);

         _isBusy = false;
         LogoutCommand.RaiseCanExecuteChanged();
         LoginCommand.RaiseCanExecuteChanged();
      }

      public void Handle(IsLoggedInMessage message)
      {
         if (IsConnected)
            _messenger.Send(new ConnectionEstablishedMessage(Profile));
         else
            _messenger.Send(new ConnectionDownMessage());
      }

      public void Handle(GetProfileDetailsResponse message)
      {
         Profile = message.Details;
         _messenger.Send(new DownloadPictureMessage(Profile.AvatarUrls.Avatar48x48));
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
            _messenger.Send(new LoggedInMessage());
            _messenger.LogMessage("Logged in successfully!", LogLevel.Info);
         }
         else
         {
            IsConnected = false;
            _messenger.Send(new LoggedOutMessage());
            _messenger.LogMessage("Failed to log in! Reason: " + message.Result.ErrorMessage, LogLevel.Warning);
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
            _messenger.LogMessage("Logged in using existing security token.");
            _messenger.Send(new LoggedInMessage());
         }
         else if (IsConnected && message.Response.IsLoggedIn == false)
         {
            IsConnected = false;
            _messenger.Send(new LoggedOutMessage());
         }
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

      public RelayCommand<PasswordBox> LoginCommand
      {
         get
         {

            return new RelayCommand<PasswordBox>(async password => await Login(password.Password), p => _isBusy == false);
         }
      }
      public RelayCommand LogoutCommand
      {
         get
         {
            return new RelayCommand(async () => await Logout(), () => _isBusy == false);
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