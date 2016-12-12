using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using JiraAssistant.Domain.Jira;
using JiraAssistant.Domain.NavigationMessages;
using JiraAssistant.Logic.Settings;
using JiraAssistant.Logic.Services.Jira;
using System;
using System.Threading.Tasks;
using System.Drawing;
using JiraAssistant.Domain;

namespace JiraAssistant.Logic.ContextlessViewModels
{
    public class JiraSessionViewModel : ViewModelBase
    {
        private readonly AssistantSettings _configuration;
        private bool _isLoggedIn;
        private RawProfileDetails _profile;
        private Bitmap _profileAvatar;
        private readonly IJiraApi _jiraApi;
        private readonly IMessenger _messenger;
		private readonly IInvokeOnUiThread _onUiThread;

		public JiraSessionViewModel(AssistantSettings configuration,
           							IMessenger messenger,
           							IJiraApi jiraApi,
		                            IInvokeOnUiThread onUiThread)
        {
			_onUiThread = onUiThread;
			_jiraApi = jiraApi;
            _configuration = configuration;
            _messenger = messenger;

            _jiraApi.Session.OnLogout += (sender, args) => LoggedOut();
            _jiraApi.Session.OnSuccessfulLogin += (sender, args) => LoggedIn();
        }

        internal void LoggedOut()
        {
            IsLoggedIn = false;
            Profile = null;
            ProfileAvatar = null;

            _messenger.Send(new ClearNavigationHistoryMessage());
        }

        internal void LoggedIn()
        {
            IsLoggedIn = true;
            _configuration.LastLogin = DateTime.Now;
            Task.Factory.StartNew(async () =>
            {
                var details = await _jiraApi.Session.GetProfileDetails();
				_onUiThread.Invoke(() =>
             {
                   Profile = details;
               });

                var avatar = await _jiraApi.DownloadPicture(details.AvatarUrls.Avatar48x48);
				_onUiThread.Invoke(() =>
             {
                   ProfileAvatar = avatar;
               });
            });
        }

		public async Task Logout()
		{
			await _jiraApi.Session.Logout();
		}

        public bool IsLoggedIn
        {
            get { return _isLoggedIn; }
            private set
            {
                _isLoggedIn = value;
                RaisePropertyChanged();
				if (IsLoggedInChanged != null)
					IsLoggedInChanged(this, _isLoggedIn);
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

        public Bitmap ProfileAvatar
        {
            get { return _profileAvatar; }
            private set
            {
                _profileAvatar = value;
                RaisePropertyChanged();
            }
        }

		public event EventHandler<bool> IsLoggedInChanged;
    }
}
