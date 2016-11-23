using Autofac;
using Gtk;

namespace JiraAssistant.Mono
{
	public partial class MainWindow : Window
	{

		public MainWindow(IComponentContext resolver) :
				base(WindowType.Toplevel)
		{
			this.Build();
			
			DeleteEvent += OnDeleteEvent;
		}

		protected void OnDeleteEvent(object sender, DeleteEventArgs a)
		{
			Application.Quit();
			a.RetVal = true;
		}
	}
}
