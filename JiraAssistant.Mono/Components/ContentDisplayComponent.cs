using System;
using Gtk;
using JiraAssistant.Logic.ContextlessViewModels;

namespace JiraAssistant.Mono.Components
{
	public class ContentDisplayComponent
	{
		private readonly ContentDisplayWidget _control;
		private readonly JiraSessionViewModel _jiraSession;

		public ContentDisplayComponent(ContentDisplayWidget control,
		                               JiraSessionViewModel jiraSession)
		{
			_control = control;
			_jiraSession = jiraSession;

			InitializeMenuBar();
		}

		private void InitializeMenuBar()
		{
			var menu = _control.Menu;

			var jiraMenu = new MenuItem("JIRA");
			var submenu = new Menu();
			var logoutItem = new MenuItem("Logout");
			logoutItem.Activated += async (sender, e) => await _jiraSession.Logout();
			submenu.Add(logoutItem);

			menu.Add(submenu);
		}
}
}
