
// This file has been generated by the GUI designer. Do not modify.
namespace JiraAssistant.Mono
{
	public partial class MainMenuWidget
	{
		private global::Gtk.UIManager UIManager;

		private global::Gtk.Action JiraAction;

		private global::Gtk.Action LogoutAction;

		protected virtual void Build()
		{
			global::Stetic.Gui.Initialize(this);
			// Widget JiraAssistant.Mono.MainMenuWidget
			Stetic.BinContainer w1 = global::Stetic.BinContainer.Attach(this);
			this.UIManager = new global::Gtk.UIManager();
			global::Gtk.ActionGroup w2 = new global::Gtk.ActionGroup("Default");
			this.JiraAction = new global::Gtk.Action("JiraAction", global::Mono.Unix.Catalog.GetString("Jira"), null, null);
			this.JiraAction.ShortLabel = global::Mono.Unix.Catalog.GetString("Jira");
			w2.Add(this.JiraAction, null);
			this.LogoutAction = new global::Gtk.Action("LogoutAction", global::Mono.Unix.Catalog.GetString("Logout"), null, null);
			this.LogoutAction.ShortLabel = global::Mono.Unix.Catalog.GetString("Logout");
			w2.Add(this.LogoutAction, null);
			this.UIManager.InsertActionGroup(w2, 0);
			this.Name = "JiraAssistant.Mono.MainMenuWidget";
			if ((this.Child != null))
			{
				this.Child.ShowAll();
			}
			w1.SetUiManager(UIManager);
			this.Hide();
		}
	}
}