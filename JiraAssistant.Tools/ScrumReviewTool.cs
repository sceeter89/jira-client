using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using JiraAssistant.Domain.Jira;
using JiraAssistant.Domain.Tools;

namespace JiraAssistant.Tools
{
	[Export(typeof(ICustomTool))]
	public class ScrumReviewTool : IJqlBasedCustomTool
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
				yield return new QueryParameter("updatedAfter", QueryParameterType.Date, label: "Last Scrum Review date");
			}
		}

		private async Task<IDictionary<string, string>> FetchEpicsNames(IList<string> epicKeys, IJiraApi jiraApi)
		{
			IDictionary<string, string> result = null;
			if (epicKeys.Any())
			{
				var lookForKeys = epicKeys.Where(key => string.IsNullOrWhiteSpace(key) == false);

				var jqlQuery = string.Format("key IN ({0})", string.Join(",", lookForKeys));

				var epics = await jiraApi.SearchForIssues(jqlQuery);

				result = epics.ToDictionary(i => i.Key, i => i.EpicName);
			}
			else
				result = new Dictionary<string, string>();
			
			result[""] = "(No Epic)";

			return result;
		}

		public async Task<IOutput> ProcessIssues(IEnumerable<JiraIssue> issues, IJiraApi jiraApi)
		{
			var resultBuilder = new StringBuilder();
			var grouped = issues.GroupBy(i => i.EpicLink);

			var epicLinks = new List<string>();

			epicLinks.AddRange(grouped.Select(group => group.Key));

			var epics = await FetchEpicsNames(epicLinks, jiraApi);

			foreach (var group in grouped)
			{
				resultBuilder.AppendLine();
				resultBuilder.AppendLine("h2. " + epics[group.Key ?? ""]);
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
