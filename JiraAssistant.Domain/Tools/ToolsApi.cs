using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

		Task<IOutput> ProcessIssues(IEnumerable<JiraIssue> issues, IDictionary<QueryParameter, string> parametersValues, IJiraApi jiraApi);
	}
}
