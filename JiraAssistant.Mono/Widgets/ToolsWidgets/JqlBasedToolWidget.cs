using System;
using System.Collections.Generic;
using System.Linq;
using Gtk;
using JiraAssistant.Domain.Tools;
using JiraAssistant.Mono.Widgets.ToolsWidgets.ParametersWidgets;

namespace JiraAssistant.Mono.Widgets.ToolsWidgets
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class JqlBasedToolWidget : Gtk.Bin
	{
		private Dictionary<QueryParameter, IQueryParameterWidget> _parametersWidgets = new Dictionary<QueryParameter, IQueryParameterWidget>();

		public JqlBasedToolWidget(IEnumerable<QueryParameter> parameters)
		{
			this.Build();
			var parametersList = parameters.ToList();

			table1.NRows = (uint)parametersList.Count;

			for (uint i = 0; i < parametersList.Count; i++)
			{
				var parameter = parametersList[(int)i];
				var label = new Label { LabelProp = parameter.Label };
				var parameterWidget = GetWidgetForParameter(parameter);

				_parametersWidgets[parameter] = parameterWidget;

				table1.Attach(label, 0, 1, i, i + 1, AttachOptions.Fill, AttachOptions.Fill, 3, 3);
				table1.Attach((Widget)parameterWidget, 1, 2, i, i + 1, AttachOptions.Fill, AttachOptions.Fill, 3, 3);
			}
			this.Child.ShowAll();
		}

		private IQueryParameterWidget GetWidgetForParameter(QueryParameter parameter)
		{
			switch (parameter.Type)
			{
				case QueryParameterType.Text:
					return new TextParameterWidget();
				default:
					throw new ArgumentException("Unsupported parameter type: " + parameter.Type);
			}
		}

		public IDictionary<QueryParameter, string> GetParametersValues()
		{
			return _parametersWidgets.Keys.ToDictionary(p => p, p => _parametersWidgets[p].GetParameterValue());
		}
	}
}
