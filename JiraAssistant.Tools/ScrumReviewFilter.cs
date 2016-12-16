using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using JiraAssistant.Domain.Jira;
using JiraAssistant.Domain.Tools;

namespace JiraAssistant.Tools
{
	[Export(typeof(ICustomTool))]
	public class ScrumReviewFilter : IJqlBasedCustomTool
	{
		public string Author
		{
			get { return "Jira Assistant"; }
		}

		public string Description
		{
			get
			{
				return @"Get tasks finished or updated in current sprint, to facilitate Scrum Review preparations.";
			}
		}

		public Guid Id
		{
			get { return new Guid("AFCB6428-7A56-4F4B-8790-3AE8A985CA8D"); }
		}

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

		public string Name
		{
			get
			{
				return "Scrum Review summary";
			}
		}

		public IEnumerable<QueryParameter> QueryParameters
		{
			get
			{
				yield return new QueryParameter("project", QueryParameterType.Text, label:"Project name or key");
				yield return new QueryParameter("updatedAfter", QueryParameterType.Text, label:"Last Scrum Review date");
			}
		}

		public IOutput ProcessIssues(IEnumerable<JiraIssue> issues)
		{
			return new FlatFileOutput { Content = "Bla bla bla" };
		}
	}
}
