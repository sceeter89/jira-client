using Gtk;
using JiraAssistant.Logic.ContextlessViewModels;
using JiraAssistant.Logic.Settings;

namespace JiraAssistant.Mono.Controllers
{
	public class ContentDisplayController
	{
		private readonly ContentDisplayWidget _control;
		private readonly JiraSessionViewModel _jiraSession;
		private readonly AssistantSettings _assistantSettings;

		public ContentDisplayController(ContentDisplayWidget control,
									   JiraSessionViewModel jiraSession,
									   AssistantSettings assistantSettings)
		{
			_control = control;
			_jiraSession = jiraSession;
			_assistantSettings = assistantSettings;

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
	}
}
