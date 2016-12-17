namespace JiraAssistant.Mono.Widgets.ToolsWidgets.ParametersWidgets
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class DateParameterWidget : Gtk.Bin, IQueryParameterWidget
	{
		public DateParameterWidget()
		{
			Build();
		}

		public string GetParameterValue()
		{
			return calendar.Date.ToString("yyyy-MM-dd 00:00");
		}
	}
}
