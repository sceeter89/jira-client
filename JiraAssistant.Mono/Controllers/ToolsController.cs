using System;
using System.Collections.Generic;
using Gtk;
using JiraAssistant.Domain.Tools;
using JiraAssistant.Logic.ContextlessViewModels;

namespace JiraAssistant.Mono.Controllers
{
	public class ToolsController
	{
		private readonly ToolsWidget _control;
		private readonly CustomToolsViewModel _customTools;

		private Dictionary<string, ICustomTool> _tools = new Dictionary<string, ICustomTool>();

		public ToolsController(ToolsWidget control,
							   CustomToolsViewModel customTools)
		{
			_control = control;
			_customTools = customTools;

			InitializeWidget();
		}

		private void InitializeWidget()
		{
			SetColumns();

			var store = new ListStore(typeof(string), typeof(string), typeof(string), typeof(string));

			foreach (var tool in _customTools.CustomTools)
			{
				var toolId = tool.Id.ToString();

				_tools[toolId] = tool;
				store.AppendValues(tool.Name, tool.Author, tool.Description, toolId);
			}

			_control.ToolsTree.Model = store;
			_control.ToolsTree.RowActivated += OnRowActivated;
		}

		private void OnRowActivated(object sender, RowActivatedArgs args)
		{
			TreeIter iter;
			TreeView view = (TreeView)sender;

			if (view.Model.GetIter(out iter, args.Path) == false)
				return;
			
			var tool = _tools[(string)view.Model.GetValue(iter, (int)Column.Id)];
		}

		private TreeViewColumn BuildColumn(Column column)
		{
			var renderer = new CellRendererText();
			var treeViewColumn = new TreeViewColumn(column.ToString(), renderer, "text", column);
			treeViewColumn.SortColumnId = (int)column;

			return treeViewColumn;
		}

		private void SetColumns()
		{
			foreach (var value in Enum.GetValues(typeof(Column)))
			{
				_control.ToolsTree.AppendColumn(BuildColumn((Column)value));
			}
		}

		private enum Column
		{
			Name,
			Author,
			Description,
			Id
		}
	}
}
