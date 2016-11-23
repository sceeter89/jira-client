using Autofac;
using JiraAssistant.Mono.Controllers;

namespace JiraAssistant.Mono
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class ToolsWidget : Gtk.Bin
	{
		private readonly ToolsController _controller;

		public ToolsWidget()
		{
			this.Build();

			_controller = Bootstrap.IocContainer.Resolve<ToolsController>(new NamedParameter("control", this));
		}
	}
}
