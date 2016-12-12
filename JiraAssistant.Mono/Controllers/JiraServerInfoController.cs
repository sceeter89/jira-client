using System.Drawing;
using System.ComponentModel;
using System.Linq;
using JiraAssistant.Logic.ContextlessViewModels;
using JiraAssistant.Logic.Settings;
using Gdk;
using System.IO;
using System.Drawing.Imaging;

namespace JiraAssistant.Mono.Controllers
{
	public class JiraServerInfoController
	{
		private readonly JiraSessionViewModel _jiraSession;
		private readonly AssistantSettings _assistantSettings;
		private readonly JiraServerInfoWidget _control;

		public JiraServerInfoController(JiraServerInfoWidget control,
									   JiraSessionViewModel jiraSession,
									   AssistantSettings assistantSettings)
		{
			_control = control;
			_jiraSession = jiraSession;
			_assistantSettings = assistantSettings;

			_jiraSession.IsLoggedInChanged += OnIsLoggedInChanged;
			_jiraSession.PropertyChanged += JiraSessionPropertyChanged;
		}

		private void OnIsLoggedInChanged(object sender, bool isLoggedIn)
		{
		}

		private void JiraSessionPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(_jiraSession.Profile))
				UpdateJiraConnectionInfo();
			else if (e.PropertyName == nameof(_jiraSession.ProfileAvatar))
				UpdateAvatar();
		}

		private void UpdateJiraConnectionInfo()
		{
			_control.JiraAddress.Text = _assistantSettings.JiraUrl;
			_control.UserName.Text = _jiraSession.Profile.DisplayName;
			_control.Email.Text = _jiraSession.Profile.EmailAddress;
			_control.Roles.Text = string.Join(", ", _jiraSession.Profile.ApplicationRoles.Items.Select(r => r.Name));
			_control.Groups.Text = string.Join(", ", _jiraSession.Profile.Groups.Items.Select(r => r.Name));
		}

		private void UpdateAvatar()
		{
			_control.Avatar.Pixbuf = BitmapToPixBuf(_jiraSession.ProfileAvatar);
		}

		Pixbuf BitmapToPixBuf(Bitmap bitmap)
		{
			using (var stream = new MemoryStream())
			{
				bitmap.Save(stream, ImageFormat.Bmp);
				stream.Position = 0;
				var result = new Pixbuf(stream);
				return result;
			}
		}

	}
}
