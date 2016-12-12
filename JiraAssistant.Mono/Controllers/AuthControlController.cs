using Gtk;
using System;
using JiraAssistant.Domain.Ui;
using JiraAssistant.Logic.ContextlessViewModels;
using JiraAssistant.Domain;

namespace JiraAssistant.Mono.Controllers
{
	public class AuthControlController
	{
		private readonly LoginPageViewModel _login;
		private readonly JiraSessionViewModel _jiraSession;
		private readonly AuthControlWidget _control;
		private readonly IInvokeOnUiThread _onUiThread;


		public AuthControlController(AuthControlWidget control,
									JiraSessionViewModel jiraSession,
									LoginPageViewModel login,
		                            IInvokeOnUiThread onUiThread)
		{
			_control = control;
			_jiraSession = jiraSession;
			_login = login;
			_onUiThread = onUiThread;

			IsLoggedInChanged(this, false);

			_jiraSession.IsLoggedInChanged += IsLoggedInChanged;
			_control.LoginAttempt += OnLoginAttempt;
			_control.JiraAddress = _login.JiraAddress;
			_control.Username = _login.Username;

			_login.PropertyChanged += (sender, e) =>
			{
				if (e.PropertyName == "LoginErrorMessage")
				{
					_control.Message = _login.LoginErrorMessage;
				}
			};
		}

		private void OnLoginAttempt(object sender, LoginAttemptEventArgs e)
		{
			_login.JiraAddress = e.JiraUrl;
			_login.Username = e.Username;
			_login.LoginCommand.Execute(new Func<string>(() => e.Password));
		}

		private void IsLoggedInChanged(object sender, bool isLoggedIn)
		{
			_onUiThread.Invoke(() =>
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
