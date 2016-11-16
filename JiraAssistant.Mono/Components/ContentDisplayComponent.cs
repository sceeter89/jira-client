using System.Linq;
using System.ComponentModel;
using GalaSoft.MvvmLight.Threading;
using Gtk;
using JiraAssistant.Logic.ContextlessViewModels;
using JiraAssistant.Logic.Settings;
using Gdk;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace JiraAssistant.Mono.Components
{
	public class ContentDisplayComponent
	{
		private readonly ContentDisplayWidget _control;
		private readonly JiraSessionViewModel _jiraSession;
		private readonly AssistantSettings _assistantSettings;

		public ContentDisplayComponent(ContentDisplayWidget control,
									   JiraSessionViewModel jiraSession,
		                               AssistantSettings assistantSettings)
		{
			_control = control;
			_jiraSession = jiraSession;
			_assistantSettings = assistantSettings;

			_jiraSession.IsLoggedInChanged += OnIsLoggedInChanged;
			_jiraSession.PropertyChanged += JiraSessionPropertyChanged;
			InitializeMenuBar();
		}

		private void InitializeMenuBar()
		{
			var menu = _control.Menu;

			var jiraMenu = new MenuItem("JIRA");
			var submenu = new Menu();
			var logoutItem = new MenuItem("Logout");
			logoutItem.Activated += async (sender, e) =>
			{
				await _jiraSession.Logout();
			};
			submenu.Add(logoutItem);
			jiraMenu.Submenu = submenu;

			menu.Add(jiraMenu);

			menu.ShowAll();
		}

		private void OnIsLoggedInChanged(object sender, bool isLoggedIn)
		{
			DispatcherHelper.CheckBeginInvokeOnUI(() =>
			{
				
			});
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
			_control.Server_JiraAddress.Text = _assistantSettings.JiraUrl;
			_control.Server_UserName.Text = _jiraSession.Profile.DisplayName;
			_control.Server_Email.Text = _jiraSession.Profile.EmailAddress;
			_control.Server_Roles.Text = string.Join(", ", _jiraSession.Profile.ApplicationRoles.Items.Select(r => r.Name));
			_control.Server_Groups.Text = string.Join(", ", _jiraSession.Profile.Groups.Items.Select(r => r.Name));
		}

		private void UpdateAvatar()
		{
			_control.Server_Avatar.Pixbuf = BitmapToPixBuf(_jiraSession.ProfileAvatar);
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
