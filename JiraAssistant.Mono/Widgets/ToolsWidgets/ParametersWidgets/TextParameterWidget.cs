using System;
namespace JiraAssistant.Mono.Widgets.ToolsWidgets.ParametersWidgets
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class TextParameterWidget : Gtk.Bin, IQueryParameterWidget
	{
		public TextParameterWidget()
		{
			this.Build();
		}

		public string GetParameterValue()
		{
			return entry1.Text;
		}
	}
}
