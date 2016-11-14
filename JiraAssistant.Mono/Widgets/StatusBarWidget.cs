using System;
namespace JiraAssistant.Mono
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class StatusBarWidget : Gtk.Bin
	{
		public StatusBarWidget()
		{
			this.Build();
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
