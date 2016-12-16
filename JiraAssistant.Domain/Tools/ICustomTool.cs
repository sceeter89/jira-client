using System;
using System.Collections.Generic;
using System.IO;
using JiraAssistant.Domain.Jira;

namespace JiraAssistant.Domain.Tools
{
	public interface ICustomTool
	{
		Guid Id { get; }

		string Name { get; }
		string Description { get; }
		string Author { get; }
	}

	public interface IJqlBasedCustomTool : ICustomTool
	{
		string JqlQuery { get; }
		IEnumerable<QueryParameter> QueryParameters { get; }

		IOutput ProcessIssues(IEnumerable<JiraIssue> issues);
	}

	public interface IOutput
	{
		void Save(string path);
	}

	public class FlatFileOutput : IOutput
	{
		public string Content { get; set; }

		public void Save(string path)
		{
			File.WriteAllText(path, Content ?? string.Empty);
		}
	}

	public class QueryParameter
	{
		public QueryParameter(string name, QueryParameterType type)
		{
			Name = name;
			Type = type;
		}

		public string Name { get; private set; }
		public QueryParameterType Type { get; private set; }
	}

	public enum QueryParameterType
	{
		Text
	}
}
