using System;
using Gtk;
using JiraAssistant.Logic.ContextlessViewModels;

namespace JiraAssistant.Mono.Components
{
	public class MenuComponent
	{
		private readonly JiraSessionViewModel _jiraSession;
		private readonly MainMenuWidget _control;

		public MenuComponent(MainMenuWidget control,
		                     JiraSessionViewModel jiraSession)
		{
			_control = control;
			_jiraSession = jiraSession;

			control.Logout.Activated += OnLogoutClicked;
		}

		private void OnLogoutClicked(object sender, EventArgs e)
		{
			
		}
	}
}
