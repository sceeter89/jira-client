using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using JiraAssistant.Model;
using JiraAssistant.Model.Jira;
using JiraAssistant.Pages;
using JiraAssistant.Services;
using JiraAssistant.Services.Resources;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace JiraAssistant.ViewModel
{
   public class JiraSessionViewModel : ViewModelBase
   {
      public EventHandler OnSuccessfulLogin;
      public EventHandler OnLogout;
      private readonly JiraSessionService _sessionService;
      private readonly AssistantConfiguration _configuration;
      private bool _isLoggedIn;
      private RawProfileDetails _profile;
      private ImageSource _profileAvatar;
      private readonly ResourceDownloader _downloader;
      private readonly INavigator _navigator;

      public JiraSessionViewModel(JiraSessionService sessionService,
         AssistantConfiguration configuration,
         ResourceDownloader downloader,
         INavigator navigator)
      {
         _sessionService = sessionService;
         _configuration = configuration;
         _downloader = downloader;
         _navigator = navigator;
         LogoutCommand = new RelayCommand(Logout, () => IsLoggedIn);
      }

      private void Logout()
      {
         _sessionService.Logout();
         IsLoggedIn = false;
         Profile = null;
         ProfileAvatar = null;

         if (OnLogout != null)
            OnLogout(this, EventArgs.Empty);

         _navigator.ClearHistory();
      }

      public void LoggedIn()
      {
         IsLoggedIn = true;
         Task.Factory.StartNew(async () =>
         {
            var details = await _sessionService.GetProfileDetails();
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
               Profile = details;
            });

            var avatar = await _downloader.DownloadPicture(details.AvatarUrls.Avatar48x48);
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
               ProfileAvatar = avatar;
            });
         });
      }

      public bool IsLoggedIn
      {
         get { return _isLoggedIn; }
         set
         {
            _isLoggedIn = value;
            RaisePropertyChanged();
            LogoutCommand.RaiseCanExecuteChanged();
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

      public ImageSource ProfileAvatar
      {
         get { return _profileAvatar; }
         set
         {
            _profileAvatar = value;
            RaisePropertyChanged();
         }
      }

      public RelayCommand LogoutCommand { get; private set; }
   }
}
