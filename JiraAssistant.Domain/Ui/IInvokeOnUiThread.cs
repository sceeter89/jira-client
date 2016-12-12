using System;
namespace JiraAssistant.Domain
{
	public interface IInvokeOnUiThread
	{
		void Invoke(Action action);
	}
}
