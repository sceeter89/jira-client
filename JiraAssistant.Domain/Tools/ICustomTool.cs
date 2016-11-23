using System.Collections.Generic;
using System.IO;
using JiraAssistant.Domain.Jira;

namespace JiraAssistant.Domain.Tools
{
	public interface ICustomTool
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
		private readonly string _fileContent;

		public FlatFileOutput(string fileContent)
		{
			_fileContent = fileContent;
		}

		public void Save(string path)
		{
			File.WriteAllText(path, _fileContent);
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
