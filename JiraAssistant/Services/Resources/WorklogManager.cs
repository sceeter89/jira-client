using RestSharp;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using JiraAssistant.Model.Jira;
using JiraAssistant.Services.Settings;
using JiraAssistant.Services.Jira;

namespace JiraAssistant.Services.Resources
{
   public class WorklogManager : BaseRestService, IJiraWorklogManager
   {
      public WorklogManager(AssistantSettings configuration)
         : base(configuration)
      {
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
