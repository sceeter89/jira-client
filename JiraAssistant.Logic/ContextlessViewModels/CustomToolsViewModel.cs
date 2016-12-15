using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Reflection;
using GalaSoft.MvvmLight;
using JiraAssistant.Domain.Tools;

namespace JiraAssistant.Logic.ContextlessViewModels
{
	public class CustomToolsViewModel : ViewModelBase
	{
		[ImportMany(typeof(ICustomTool))]
		private IList<ICustomTool> _customTools = new List<ICustomTool>();

		public CustomToolsViewModel()
		{
			var jiraAssistantPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			var pluginsPath = Path.Combine(jiraAssistantPath, "Plugins");

			if (Directory.Exists(pluginsPath) == false)
				Directory.CreateDirectory(pluginsPath);

			var catalog = new AggregateCatalog();
			catalog.Catalogs.Add(new DirectoryCatalog(pluginsPath));
			catalog.Catalogs.Add(new DirectoryCatalog(jiraAssistantPath));

			var container = new CompositionContainer(catalog);

			container.ComposeParts(this);
		}

		public IList<ICustomTool> CustomTools
		{
			get
			{
				return _customTools;
			}
		}
	}
}
