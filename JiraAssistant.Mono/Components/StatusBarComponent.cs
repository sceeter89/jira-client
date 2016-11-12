using System;
using Gtk;
using JiraAssistant.Logic.ContextlessViewModels;

namespace JiraAssistant.Mono
{
	public class StatusBarComponent
	{
		public StatusBarComponent(StatusBarWidget control,
		                          JiraSessionViewModel jiraSession)
		{
			jiraSession.IsLoggedInChanged += IsLoggedInChanged;
		}

		private void IsLoggedInChanged(object sender, bool e)
		{

		}
	}
}
