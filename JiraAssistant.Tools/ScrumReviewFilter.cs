using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

		public Version Version
		{
			get { return new Version(1, 0); }
		}

		public string Description
		{
			get
			{
				return 
@"Get tasks finished or updated in current sprint, to facilitate Scrum Review preparations.
Result is exported as Confluence Markup.";
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
				yield return new QueryParameter("project", QueryParameterType.Text, label: "Project name or key");
				yield return new QueryParameter("updatedAfter", QueryParameterType.Text, label: "Last Scrum Review date");
			}
		}

		public IOutput ProcessIssues(IEnumerable<JiraIssue> issues, IJiraApi jiraApi)
		{
			var resultBuilder = new StringBuilder();
			var grouped = issues.GroupBy(i => i.EpicLink);

			var epicLinks = new List<string>();

			epicLinks.AddRange(grouped.Select(group => group.Key));

			foreach (var group in grouped)
			{
				resultBuilder.AppendLine();
				if (string.IsNullOrWhiteSpace(group.Key))
					resultBuilder.AppendLine("h2. (No Epic)");
				else
					resultBuilder.AppendLine("h2. " + group.Key);
				resultBuilder.AppendLine();

				foreach (var issue in group)
					resultBuilder.AppendLine(string.Format("* *{0}* - {1}", issue.Key, EscapeConfluenceMarkupCharacters(issue.Summary)));
			}

			return new FlatTextOutput
			{
				SuggestedFilename = "Issues List.txt",
				Content = resultBuilder.ToString()
			};
		}

        private string EscapeConfluenceMarkupCharacters(string summary)
		{
			return Regex.Replace(summary, @"[{\[\]\}\(\)!@\\]", m => "\\" + m.Value);
		}
	}
}
