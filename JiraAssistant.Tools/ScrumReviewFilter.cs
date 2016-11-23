using System.Collections.Generic;
using System.ComponentModel.Composition;
using JiraAssistant.Domain.Jira;
using JiraAssistant.Domain.Tools;

namespace JiraAssistant.Tools
{
	[Export(typeof(ICustomTool))]
	public class ScrumReviewFilter : ICustomTool
	{
		public string JqlQuery
		{
			get
			{
				return @"
				project = '{project}' AND updated >= '{updatedAfter}'
				AND issuetype NOT IN (Epic) AND status IN (Closed, 'Awaiting Review')
				";
			}
		}

		public IEnumerable<QueryParameter> QueryParameters
		{
			get
			{
				yield return new QueryParameter("project", QueryParameterType.Text);
				yield return new QueryParameter("updatedAfter", QueryParameterType.Text);
			}
		}

		public IOutput ProcessIssues(IEnumerable<JiraIssue> issues)
		{
			return new FlatFileOutput("Bla bla bla");
		}
	}
}
