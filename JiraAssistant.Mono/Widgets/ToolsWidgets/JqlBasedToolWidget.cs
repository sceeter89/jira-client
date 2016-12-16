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

			foreach (var parameter in parameters)
			{
				var layout = new HBox(false, 3) { HeightRequest = 35 };
				var parameterWidget = GetWidgetForParameter(parameter);

				_parametersWidgets[parameter] = parameterWidget;

				layout.PackStart(new Label { LabelProp = parameter.Label }, false, true, 5);
				layout.PackEnd((Widget)parameterWidget, true, true, 5);

				vbox1.PackEnd(layout, false, true, 5);
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
