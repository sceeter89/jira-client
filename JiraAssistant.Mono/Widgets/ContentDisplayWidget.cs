using Autofac;
using Gtk;
using JiraAssistant.Mono.Controllers;

namespace JiraAssistant.Mono
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class ContentDisplayWidget : Gtk.Bin
	{
		private readonly ContentDisplayController _controller;

		public ContentDisplayWidget()
		{
			this.Build();

			_controller = Bootstrap.IocContainer.Resolve<ContentDisplayController>(new NamedParameter("control", this));
		}

		public MenuBar Menu
		{
			get
			{
				return mainMenu;
			}
		}
	}
}
