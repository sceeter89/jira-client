using RestSharp;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using JiraAssistant.Domain.Jira;
using JiraAssistant.Logic.Settings;
using Newtonsoft.Json;

namespace JiraAssistant.Logic.Services.Resources
{
	public class WorklogManager : BaseRestService, IJiraWorklogManager
	{
		public WorklogManager(AssistantSettings configuration)
		   : base(configuration)
		{
		}

		public async Task<RawWorklogList> GetWorklog(string issueKey)
		{
			var client = BuildRestClient();

			var logWorkRequest = new RestRequest("/rest/api/2/issue/{key}/worklog", Method.GET);
			logWorkRequest.AddUrlSegment("key", issueKey);

			var response = await client.ExecuteTaskAsync(logWorkRequest);
			var result = JsonConvert.DeserializeObject<RawWorklogList>(response.Content);

			return result;
		}

		public async Task Log(JiraIssue issue, double hoursSpent)
		{
			var client = BuildRestClient();

			var logWorkRequest = new RestRequest("/rest/api/2/issue/{key}/worklog", Method.POST);
			logWorkRequest.AddUrlSegment("key", issue.Key);
			logWorkRequest.RequestFormat = DataFormat.Json;
			logWorkRequest.AddJsonBody(new Dictionary<string, string>
			{
			   {"started", DateTime.Now.ToString("yyyy-MM-ddT00:0:0.0+0000") },
			   {"timeSpentSeconds", ((int)(hoursSpent * 3600)).ToString() }
			});

			var response = await client.ExecuteTaskAsync(logWorkRequest);
		}
	}
}
