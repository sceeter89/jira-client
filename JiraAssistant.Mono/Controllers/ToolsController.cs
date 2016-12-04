using JiraAssistant.Logic;
using JiraAssistant.Logic.ContextlessViewModels;

namespace JiraAssistant.Mono.Controllers
{
	public class ToolsController
	{
		private readonly ToolsWidget _control;

		public ToolsController(ToolsWidget control)
		{
			_control = control;
		}
	}
}
