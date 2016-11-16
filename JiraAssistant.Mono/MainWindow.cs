using Autofac;
using Gtk;
using JiraAssistant.Mono.Components;

namespace JiraAssistant.Mono
{
	public partial class MainWindow : Window
	{
		private readonly StatusBarComponent _statusBarComponent;
		private readonly AuthControlComponent _authControlComponent;
		private readonly ContentDisplayComponent _contentDisplayComponent;

		public MainWindow(IComponentContext resolver) :
				base(WindowType.Toplevel)
		{
			this.Build();

			_statusBarComponent = resolver.Resolve<StatusBarComponent>(new NamedParameter("control", statusBarWidget));
			_authControlComponent = resolver.Resolve<AuthControlComponent>(new NamedParameter("control", authControlWidget));
			_contentDisplayComponent = resolver.Resolve<ContentDisplayComponent>(new NamedParameter("control", authControlWidget.ContentDisplay));
			
			DeleteEvent += OnDeleteEvent;
		}

		protected void OnDeleteEvent(object sender, DeleteEventArgs a)
		{
			Application.Quit();
			a.RetVal = true;
		}
	}
}
