using Gtk;

namespace JiraAssistant.Mono
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class ContentDisplayWidget : Gtk.Bin
	{
		public ContentDisplayWidget()
		{
			this.Build();
		}

		public MenuBar Menu
		{
			get
			{
				return mainMenu;
			}
		}

		/* JIRA Connection tab controls */
		public Entry Server_JiraAddress { get { return server_jiraAddress; } }
		public Entry Server_UserName { get { return server_userName; } }
		public Entry Server_Groups { get { return server_groups; } }
		public Entry Server_Roles { get { return server_roles; } }
		public Entry Server_Email { get { return server_email; } }
		public Image Server_Avatar { get { return server_avatar; } }
	}
}
