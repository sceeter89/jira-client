using System;
using System.Collections.Generic;
using JiraAssistant.Domain.Jira;

namespace JiraAssistant.Domain.Tools
{
	public interface ICustomTool
	{
		Guid Id { get; }
		Version Version { get; }

		string Name { get; }
		string Description { get; }
		string Author { get; }
	}

	public interface IJqlBasedCustomTool : ICustomTool
	{
		string JqlQuery { get; }
		IEnumerable<QueryParameter> QueryParameters { get; }

		IOutput ProcessIssues(IEnumerable<JiraIssue> issues, IJiraApi jiraApi);
	}

	public interface IOutput
	{
	}

	public class FlatTextOutput : IOutput
	{
		public string SuggestedFilename { get; set; }
		public string Content { get; set; }
	}

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
		Text
	}
}
