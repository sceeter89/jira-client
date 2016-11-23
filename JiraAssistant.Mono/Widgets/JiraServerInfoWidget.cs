using Autofac;
using Gtk;
using JiraAssistant.Mono.Controllers;

namespace JiraAssistant.Mono
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class JiraServerInfoWidget : Bin
	{
		private readonly JiraServerInfoController _controller;
		public JiraServerInfoWidget()
		{
			this.Build();

			_controller = Bootstrap.IocContainer.Resolve<JiraServerInfoController>(new NamedParameter("control", this));
		}

		public Entry JiraAddress { get { return jiraAddress; } }
		public Entry UserName { get { return userName; } }
		public Entry Groups { get { return groups; } }
		public Entry Roles { get { return roles; } }
		public Entry Email { get { return email; } }
		public Image Avatar { get { return avatar; } }
	}
}
