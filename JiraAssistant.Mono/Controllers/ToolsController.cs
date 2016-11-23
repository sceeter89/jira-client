using JiraAssistant.Logic;
using JiraAssistant.Logic.ContextlessViewModels;

namespace JiraAssistant.Mono.Controllers
{
	public class ToolsController
	{
		private readonly ToolsWidget _control;
		private readonly CustomToolsViewModel _customTools;

		public ToolsController(ToolsWidget control,
							   CustomToolsViewModel customTools)
		{
			_control = control;
			_customTools = customTools;
		}
	}
}
