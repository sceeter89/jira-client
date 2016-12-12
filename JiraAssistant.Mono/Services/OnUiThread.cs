using System;
using JiraAssistant.Domain;

namespace JiraAssistant.Mono
{
	public class OnUiThread : IInvokeOnUiThread
	{
		public void Invoke(Action action)
		{
			Gtk.Application.Invoke(delegate {
				action();
			});
		}
	}
}
