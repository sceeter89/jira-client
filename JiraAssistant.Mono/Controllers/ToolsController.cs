using System;
using System.Collections.Generic;
using System.Linq;
using Gtk;
using JiraAssistant.Domain;
using JiraAssistant.Domain.Tools;
using JiraAssistant.Logic.ContextlessViewModels;
using JiraAssistant.Domain.Jira;
using JiraAssistant.Mono.Widgets.ToolsWidgets;
using JiraAssistant.Mono.Windows;

namespace JiraAssistant.Mono.Controllers
{
	public class ToolsController
	{
		private readonly ToolsWidget _control;
		private readonly CustomToolsViewModel _customTools;
		private Widget _currentControl;
		private ICustomTool _currentTool;

		private Dictionary<string, ICustomTool> _tools = new Dictionary<string, ICustomTool>();
		private Dictionary<ICustomTool, Widget> _toolWidgetCache = new Dictionary<ICustomTool, Widget>();

		private readonly IJiraApi _jiraApi;
		private readonly IInvokeOnUiThread _onUiThread;

		public ToolsController(ToolsWidget control,
							   CustomToolsViewModel customTools,
							   IJiraApi jiraApi,
							   IInvokeOnUiThread onUiThread)
		{
			_control = control;
			_customTools = customTools;
			_jiraApi = jiraApi;
			_onUiThread = onUiThread;

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
			_control.ToolsTree.Selection.UnselectAll();
			_control.RunButton.Clicked += OnRunClicked;
		}

		private void HandleToolOutput(IOutput toolOutput)
		{
			if (toolOutput.GetType() == typeof(FlatTextOutput))
			{
				var textualOutput = (FlatTextOutput)toolOutput;
				var textPreviewDialog = new TextPreviewDialog(textualOutput.Content, textualOutput.SuggestedFilename);

				textPreviewDialog.Run();
				textPreviewDialog.Destroy();
				return;
			}

			throw new ArgumentException("Unsupported output type: " + toolOutput.GetType().FullName);
		}

		private async void OnRunClicked(object sender, EventArgs args)
		{
			_control.RunButton.Sensitive = false;

			try
			{
				var toolInterfaces = _currentTool.GetType().GetInterfaces();
				if (toolInterfaces.Contains(typeof(IJqlBasedCustomTool)))
				{
					var jqlBasedTool = (IJqlBasedCustomTool)_currentTool;
					var jqlBasedToolWidget = (JqlBasedToolWidget)_currentControl;

					var parameters = jqlBasedToolWidget.GetParametersValues();

					var query = jqlBasedTool.JqlQuery;

					foreach (var parameter in parameters)
						query = query.Replace("{" + parameter.Key.Name + "}", parameter.Value);

					var issues = await _jiraApi.SearchForIssues(query);

					var output = await jqlBasedTool.ProcessIssues(issues, _jiraApi);

					_onUiThread.Invoke(() => HandleToolOutput(output));
				}
			}
			catch (Exception e)
			{
				_onUiThread.Invoke(() =>
				{
					var d = new MessageDialog(Bootstrap.MainWindow,
											  DialogFlags.Modal,
											  MessageType.Error,
											  ButtonsType.Close,
					                          e.Message.Replace("{", "{{"));
					d.Run();
					d.Destroy();
				});
			}
			finally
			{
				_control.RunButton.Sensitive = true;
			}
		}

		private void OnRowActivated(object sender, RowActivatedArgs args)
		{
			TreeIter iter;
			var view = (TreeView)sender;

			if (view.Model.GetIter(out iter, args.Path) == false)
				return;

			var tool = _tools[(string)view.Model.GetValue(iter, (int)Column.Id)];

			var toolWidget = PrepareWidgetForTool(tool);

			if (toolWidget == _currentControl)
				return;

			_currentControl = toolWidget;
			_currentTool = tool;

			foreach (var child in _control.ControlBox.Children)
				_control.ControlBox.Remove(child);

			_control.ControlBox.PackStart(toolWidget, true, false, 4);

			_control.RunButton.Sensitive = true;
		}

		private Widget PrepareWidgetForTool(ICustomTool tool)
		{
			if (_toolWidgetCache.ContainsKey(tool))
				return _toolWidgetCache[tool];

			var toolInterfaces = tool.GetType().GetInterfaces();
			Widget widget = null;
			if (toolInterfaces.Contains(typeof(IJqlBasedCustomTool)))
			{
				var jqlBasedTool = (IJqlBasedCustomTool)tool;
				widget = new JqlBasedToolWidget(jqlBasedTool.QueryParameters);
			}

			_toolWidgetCache[tool] = widget;

			return widget;
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
