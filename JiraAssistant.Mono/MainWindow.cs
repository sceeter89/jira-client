using System;
namespace JiraAssistant.Mono
{
	public partial class MainWindow : Gtk.Window
	{
		public MainWindow() :
				base(Gtk.WindowType.Toplevel)
		{
			this.Build();
		}
	}
}
