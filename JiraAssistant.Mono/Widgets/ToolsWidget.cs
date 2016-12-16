using Autofac;
using Gtk;
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

		public TreeView ToolsTree
		{
			get { return treeview1; }
		}

		public VBox ControlBox
		{
			get { return vbox2; }
		}

		public Button RunButton
		{
			get { return button1; }
		}
	}
}
