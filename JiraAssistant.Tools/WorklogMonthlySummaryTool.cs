using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;
using System.Threading.Tasks;
using JiraAssistant.Domain.Jira;
using JiraAssistant.Domain.Tools;

namespace JiraAssistant.Tools
{
	[Export(typeof(ICustomTool))]
	public class WorklogMonthlySummaryTool : IJqlBasedCustomTool
	{
		private QueryParameter periodStartParameter = new QueryParameter("periodStart", QueryParameterType.Date, "First day of inspected month", "If you want to get report for November 2016 then select November 1st 2016.");

		public string Author { get { return "Jira Assistant"; } }

		public string Description { get { return @"Sum up how much time users spent on tasks in given month."; } }

		public Guid Id { get { return new Guid("336416e0-c53c-11e6-a4a6-cec0c932ce01"); } }

		public string JqlQuery { get { return "project in ({projects}) AND updated >= '{periodStart}'"; } }

		public string Name { get { return "Monthly worklog summary"; } }

		public IEnumerable<QueryParameter> QueryParameters
		{
			get
			{
				yield return new QueryParameter("projects", QueryParameterType.Text, "List of projects", "Comma seprated list of projects' keys or names");
				yield return periodStartParameter;
			}
		}

		public Version Version { get { return new Version(1, 0); } }

		public async Task<IOutput> ProcessIssues(IEnumerable<JiraIssue> issues, IDictionary<QueryParameter, string> parametersValues, IJiraApi jiraApi)
		{
			var timeSpentPerUser = new Dictionary<string, int>();
			var detailedSummary = new Dictionary<string, Dictionary<string, int>>();

			var startMonth = parametersValues[periodStartParameter].Substring(0, 7);
			foreach (var issue in issues)
			{
				var worklogs = await jiraApi.Worklog.GetWorklog(issue.Key);

				foreach (var worklog in worklogs.worklogs)
				{
					if (worklog.created.StartsWith(startMonth, StringComparison.InvariantCulture) == false)
						continue;
					if (detailedSummary.ContainsKey(worklog.author.DisplayName) == false)
						detailedSummary[worklog.author.DisplayName] = new Dictionary<string, int>();
					if (detailedSummary[worklog.author.DisplayName].ContainsKey(issue.Key) == false)
						detailedSummary[worklog.author.DisplayName][issue.Key] = 0;
					if (timeSpentPerUser.ContainsKey(worklog.author.DisplayName) == false)
						timeSpentPerUser[worklog.author.DisplayName] = 0;

					timeSpentPerUser[worklog.author.DisplayName] += worklog.timeSpentSeconds;
					detailedSummary[worklog.author.DisplayName][issue.Key] += worklog.timeSpentSeconds;
				}
			}

			var resultBuilder = new StringBuilder();

			foreach (var entry in timeSpentPerUser)
			{
				var hoursSpent = Math.Round(TimeSpan.FromSeconds(entry.Value).TotalHours, 2);
				resultBuilder.AppendLine(string.Format("{0}\t{1:0.00}", entry.Key, hoursSpent));
			}

			var detailsBuilder = new StringBuilder();

			foreach (var user in detailedSummary)
			{
				detailsBuilder.AppendFormat("\n{0}:\n", user.Key);

				foreach (var issue in user.Value)
				{
					var hoursSpent = Math.Round(TimeSpan.FromSeconds(issue.Value).TotalHours, 2);
					detailsBuilder.AppendFormat("* {0} = {1:0.00}\n", issue.Key, hoursSpent);
				}
			}

			return new FlatTextOutput { Content = resultBuilder + "\n\n\n" + detailsBuilder, SuggestedFilename = "Worklog summary.tsv" };
		}
	}
}
