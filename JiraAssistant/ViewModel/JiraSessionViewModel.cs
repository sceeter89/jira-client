using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Threading;
using JiraAssistant.Model.Jira;
using JiraAssistant.Services;
using JiraAssistant.Services.Jira;
using JiraAssistant.Services.Settings;
using System;
using System.Threading.Tasks;
using System.Windows.Media;

namespace JiraAssistant.ViewModel
{
   public class JiraSessionViewModel : ViewModelBase
   {
      private readonly AssistantSettings _configuration;
      private bool _isLoggedIn;
      private RawProfileDetails _profile;
      private ImageSource _profileAvatar;
      private readonly INavigator _navigator;
      private readonly IJiraApi _jiraApi;

      public JiraSessionViewModel(AssistantSettings configuration,
         INavigator navigator,
         IJiraApi jiraApi)
      {
         _jiraApi = jiraApi;
         _configuration = configuration;
         _navigator = navigator;

         _jiraApi.Session.OnLogout += (sender, args) => LoggedOut();
         _jiraApi.Session.OnSuccessfulLogin += (sender, args) => LoggedIn();
      }

      internal void LoggedOut()
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
            var details = await _jiraApi.Session.GetProfileDetails();
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
               Profile = details;
            });

            var avatar = await _jiraApi.DownloadPicture(details.AvatarUrls.Avatar48x48);
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
