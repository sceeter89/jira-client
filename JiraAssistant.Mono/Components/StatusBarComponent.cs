using System;
using Gtk;
using JiraAssistant.Logic.ContextlessViewModels;

namespace JiraAssistant.Mono.Components
{
	public class StatusBarComponent
	{
		private readonly StatusBarWidget _control;
		private readonly JiraSessionViewModel _jiraSession;

		public StatusBarComponent(StatusBarWidget control,
		                          JiraSessionViewModel jiraSession)
		{
			_control = control;
			_jiraSession = jiraSession;

			_jiraSession.IsLoggedInChanged += IsLoggedInChanged;
		}

		private void IsLoggedInChanged(object sender, bool isLoggedIn)
		{
			if (isLoggedIn)
			{
				_control.ConnectionStatus = "Online";
			}
			else
			{
				_control.ConnectionStatus = "Offline";
			}
		}
	}
}
