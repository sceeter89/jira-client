namespace JiraAssistant.Domain.Tools
{
	public interface IOutput
	{
	}

	public class FlatTextOutput : IOutput
	{
		public string SuggestedFilename { get; set; }
		public string Content { get; set; }
	}
}