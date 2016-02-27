using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using JiraAssistant.Model;
using JiraAssistant.Model.Jira;
using JiraAssistant.Pages;
using JiraAssistant.Services;
using JiraAssistant.Services.Resources;
using JiraAssistant.Services.Settings;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace JiraAssistant.ViewModel
{
   public class JiraSessionViewModel : ViewModelBase
   {
      private readonly JiraSessionService _sessionService;
      private readonly AssistantSettings _configuration;
      private bool _isLoggedIn;
      private RawProfileDetails _profile;
      private ImageSource _profileAvatar;
      private readonly ResourceDownloader _downloader;
      private readonly INavigator _navigator;

      public JiraSessionViewModel(JiraSessionService sessionService,
         AssistantSettings configuration,
         ResourceDownloader downloader,
         INavigator navigator)
      {
         _sessionService = sessionService;
         _configuration = configuration;
         _downloader = downloader;
         _navigator = navigator;

         _sessionService.OnLogout += (sender, args) => LoggedOut();
         _sessionService.OnSuccessfulLogin += (sender, args) => LoggedIn();
      }

      private void LoggedOut()
      {
         IsLoggedIn = false;
         Profile = null;
         ProfileAvatar = null;

         _navigator.ClearHistory();
      }

      internal void LoggedIn()
      {
         IsLoggedIn = true;
         _configuration.LastLogin = DateTime.Now;
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
         private set
         {
            _isLoggedIn = value;
            RaisePropertyChanged();
         }
      }

      public RawProfileDetails Profile
      {
         get { return _profile; }
         private set
         {
            _profile = value;
            RaisePropertyChanged();
         }
      }

      public ImageSource ProfileAvatar
      {
         get { return _profileAvatar; }
         private set
         {
            _profileAvatar = value;
            RaisePropertyChanged();
         }
      }
   }
}
