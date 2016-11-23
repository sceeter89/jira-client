﻿using Gtk;

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
	}
}
