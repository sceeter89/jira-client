using System;
using Gtk;

namespace JiraAssistant.Mono
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class MainMenuWidget : Gtk.Bin
	{
		public MainMenuWidget()
		{
			this.Build();
		}

		public Gtk.Action Logout
		{
			get
			{
				return LogoutAction;
			}
		}
	}
}
