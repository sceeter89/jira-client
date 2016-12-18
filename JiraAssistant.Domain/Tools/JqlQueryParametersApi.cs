namespace JiraAssistant.Domain.Tools
{
	public class QueryParameter
	{
		public QueryParameter(string name, QueryParameterType type, string label = null, string hint = "")
		{
			Name = name;
			Type = type;
			Hint = hint;
			Label = label ?? name;
		}

		public string Name { get; private set; }
		public QueryParameterType Type { get; private set; }
		public string Hint { get; private set; }
		public string Label { get; private set; }
	}

	public enum QueryParameterType
	{
		Text, Date
	}
}
