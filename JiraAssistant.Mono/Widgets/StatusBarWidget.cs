using System;
using Autofac;
using JiraAssistant.Mono.Controllers;

namespace JiraAssistant.Mono
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class StatusBarWidget : Gtk.Bin
	{
		StatusBarController _controller;

		public StatusBarWidget()
		{
			this.Build();

			_controller = Bootstrap.IocContainer.Resolve<StatusBarController>(new NamedParameter("control", this));
		}

		public string ConnectionStatus
		{
			get
			{
				return connectionStatus.Text;
			}
			set
			{
				connectionStatus.Text = value;
			}
		}
	}
}
