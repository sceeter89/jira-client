using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JiraAssistant.Domain.Jira;
using JiraAssistant.Domain.Tools;

namespace JiraAssistant.Tools
{
	public class WorklogMonthlySummaryTool : IJqlBasedCustomTool
	{
		public string Author { get { return "Jira Assistant"; } }

		public string Description { get { return @"Sum up how much time users spent on tasks in given month."; } }

		public Guid Id { get { return new Guid("336416e0-c53c-11e6-a4a6-cec0c932ce01"); } }

		public string JqlQuery
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public string Name { get { return "Monthly worklog summary"; } }

		public IEnumerable<QueryParameter> QueryParameters
		{
			get
			{
				yield return new QueryParameter("projects", QueryParameterType.Text, "List of projects", "Comma seprated list of projects' keys or names");
				yield return new QueryParameter("periodStart");
			}
		}

		public Version Version
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public Task<IOutput> ProcessIssues(IEnumerable<JiraIssue> issues, IJiraApi jiraApi)
		{
			throw new NotImplementedException();
		}
	}
}
