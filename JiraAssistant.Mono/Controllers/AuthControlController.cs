using System;
using GalaSoft.MvvmLight.Threading;
using JiraAssistant.Logic.ContextlessViewModels;

namespace JiraAssistant.Mono.Controllers
{
	public class AuthControlController
	{
		private readonly LoginPageViewModel _login;
		private readonly JiraSessionViewModel _jiraSession;
		private readonly AuthControlWidget _control;

		public AuthControlController(AuthControlWidget control,
									JiraSessionViewModel jiraSession,
									LoginPageViewModel login)
		{
			_control = control;
			_jiraSession = jiraSession;
			_login = login;

			IsLoggedInChanged(this, false);

			_jiraSession.IsLoggedInChanged += IsLoggedInChanged;
			_control.LoginAttempt += OnLoginAttempt;
			_control.JiraAddress = _login.JiraAddress;
			_control.Username = _login.Username;
		}

		private void OnLoginAttempt(object sender, LoginAttemptEventArgs e)
		{
			_login.JiraAddress = e.JiraUrl;
			_login.Username = e.Username;
			_login.LoginCommand.Execute(new Func<string>(() => e.Password));
		}

		private void IsLoggedInChanged(object sender, bool isLoggedIn)
		{
			DispatcherHelper.CheckBeginInvokeOnUI(() =>
			{
				if (isLoggedIn)
				{
					_control.IsLoginControlVisible = false;
					_control.IsContentDisplayVisible = true;
				}
				else
				{
					_control.IsLoginControlVisible = true;
					_control.IsContentDisplayVisible = false;
				}
			});
		}
	}
}
