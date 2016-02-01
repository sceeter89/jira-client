using JiraAssistant.Model;
using JiraAssistant.Model.Exceptions;
using JiraAssistant.Model.Jira;
using JiraAssistant.ViewModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Media;

namespace JiraAssistant.Services.Resources
{
   public class IssuesFinder : BaseRestService
   {
      private IDictionary<string, RawFieldDefinition> _fields;
      private readonly MetadataRetriever _metadata;
      private readonly BackgroundJobStatusViewModel _jobStatus;

      public IssuesFinder(AssistantConfiguration configuration,
         MetadataRetriever metadata,
         BackgroundJobStatusViewModel jobStatus)
         : base(configuration)
      {
         _metadata = metadata;
         _jobStatus = jobStatus;
      }

      public async Task<IEnumerable<JiraIssue>> Search(string jqlQuery)
      {
         _jobStatus.StartNewJob("Searching for issues...");

         var searchResults = new List<RawIssue>();
         var client = BuildRestClient();
         var request = new RestRequest("/rest/api/latest/search", Method.POST);

         do
         {
            request.AddJsonBody(new
            {
               jql = jqlQuery,
               startAt = 0,
               maxResults = 500,
               fields = new string[] { "*all" }
            });
            var response = await client.ExecuteTaskAsync(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
               throw new SearchFailedException(string.Format("Search request failed with invalid response code: {0}.\r\nResponse content is: {1}", response.StatusCode, response.Content));
            }

            var batch = await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<RawSearchResults>(response.Content));
            foreach (var issue in batch.Issues)
            {
               searchResults.Add(issue);
            }
            if (searchResults.Count >= batch.Total)
               break;

         } while (true);

         if (_fields == null)
         {
            _fields = (await _metadata.GetFieldsDefinitions()).ToDictionary(d => d.Name, d => d);
         }

         return ConvertIssuesToDomainModel(searchResults);
      }

      private ICollection<JiraIssue> ConvertIssuesToDomainModel(IEnumerable<RawIssue> issues)
      {
         return issues.Select(Convert).ToList();
      }

      private JiraIssue Convert(RawIssue issue)
      {
         return new JiraIssue
         {
            Key = issue.Key,
            Project = issue.BuiltInFields.Project.Name,
            Summary = issue.BuiltInFields.Summary,
            Priority = issue.BuiltInFields.Priority.Name,
            StoryPoints = GetFieldByName<float?>(issue, "Story Points") ?? 0,
            Subtasks = issue.BuiltInFields.Subtasks.Count(),
            Created = issue.BuiltInFields.Created,
            Resolved = issue.BuiltInFields.ResolutionDate ?? DateTime.MinValue,
            Status = issue.BuiltInFields.Status.Name,
            Description = issue.BuiltInFields.Description,
            Assignee = (issue.BuiltInFields.Assignee ?? RawUserInfo.EmptyInfo).DisplayName,
            Reporter = (issue.BuiltInFields.Reporter ?? RawUserInfo.EmptyInfo).DisplayName,
            CategoryColor = Colors.White,
            BuiltInFields = issue.BuiltInFields
         };
      }

      private T GetFieldByName<T>(RawIssue issue, string fieldName, string path = null)
      {
         if (_fields.ContainsKey(fieldName) == false)
            return default(T);

         var fieldId = _fields[fieldName].Id;
         JToken token;
         if (path == null)
            return issue.RawFields.Value<T>(fieldId);
         else
         {
            token = issue.RawFields.SelectToken(fieldId);
            foreach (var part in path.Split('/'))
            {
               token = token.SelectToken(part);
            }
         }

         if (token == null)
            return default(T);

         return token.ToObject<T>();
      }
   }
}
