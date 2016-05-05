using JiraAssistant.Model.Exceptions;
using JiraAssistant.Model.Jira;
using JiraAssistant.Services.Jira;
using JiraAssistant.Settings;
using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JiraAssistant.Services.Resources
{
   public class JiraAgileService : BaseRestService, IJiraAgileApi
   {
      public JiraAgileService(AssistantSettings configuration)
         : base(configuration)
      {
      }

      public async Task<RawAgileSprint> GetAgileSprintDetails(int sprintId)
      {
         var client = BuildRestClient();
         var request = new RestRequest("/rest/agile/latest/sprint/{id}", Method.GET);
         request.AddUrlSegment("id", sprintId.ToString());

         var response = await client.ExecuteTaskAsync(request);
         var result = JsonConvert.DeserializeObject<RawAgileSprint>(response.Content);

         return result;
      }

      public async Task<IEnumerable<RawAgileSprint>> GetSprints(int boardId)
      {
         var client = BuildRestClient();
         var request = new RestRequest("/rest/agile/latest/board/{id}/sprint", Method.GET);
         request.AddQueryParameter("maxResults", "500");
         request.AddQueryParameter("startAt", "0");
         request.AddUrlSegment("id", boardId.ToString());

         IRestResponse response;
         RawAgileSprintsList result;
         var allSprints = new List<RawAgileSprint>();
         do
         {
            request.Parameters[1].Value = allSprints.Count;
            response = await client.ExecuteTaskAsync(request);
            result = JsonConvert.DeserializeObject<RawAgileSprintsList>(response.Content);
            if (result.Values == null)
               return allSprints;

            allSprints.AddRange(result.Values);
         } while (result.IsLast == false);

         return allSprints;
      }

      public async Task<IEnumerable<string>> GetIssuesInSprint(int boardId, int sprintId)
      {
         var client = BuildRestClient();
         var request = new RestRequest("/rest/agile/latest/board/{boardId}/sprint/{sprintId}/issue?fields=key", Method.GET);
         request.AddQueryParameter("maxResults", "500");
         request.AddQueryParameter("startAt", "0");
         request.AddUrlSegment("boardId", boardId.ToString());
         request.AddUrlSegment("sprintId", sprintId.ToString());

         IRestResponse response;
         RawAgileSprintAssignments result;
         var allIssues = new List<string>();
         do
         {
            request.Parameters[1].Value = allIssues.Count;
            response = await client.ExecuteTaskAsync(request);
            result = JsonConvert.DeserializeObject<RawAgileSprintAssignments>(response.Content);
            allIssues.AddRange(result.Issues.Select(i => i.Key));
         } while (result.Total < allIssues.Count);

         return allIssues;
      }

      public async Task<IEnumerable<RawAgileEpic>> GetEpics(int boardId)
      {
         var client = BuildRestClient();
         var request = new RestRequest("/rest/agile/latest/board/{id}/epic", Method.GET);
         request.AddQueryParameter("maxResults", "500");
         request.AddQueryParameter("startAt", "0");
         request.AddUrlSegment("id", boardId.ToString());

         IRestResponse response;
         RawAgileEpicsList result;
         var allEpics = new List<RawAgileEpic>();
         do
         {
            request.Parameters[1].Value = allEpics.Count;
            response = await client.ExecuteTaskAsync(request);
            result = JsonConvert.DeserializeObject<RawAgileEpicsList>(response.Content);
            allEpics.AddRange(result.Values);
         } while (result.IsLast == false);

         return allEpics;
      }

      public async Task<IEnumerable<RawAgileBoard>> GetAgileBoards()
      {
         var client = BuildRestClient();
         var request = new RestRequest("/rest/agile/latest/board", Method.GET);
         request.AddQueryParameter("maxResults", "500");
         request.AddQueryParameter("startAt", "0");
         var allBoards = new List<RawAgileBoard>();
         IRestResponse response;
         RawAgileBoardsList result;
         do
         {
            request.Parameters[1].Value = allBoards.Count;
            response = await client.ExecuteTaskAsync(request);
            result = JsonConvert.DeserializeObject<RawAgileBoardsList>(response.Content);
            if (result.Values == null)
            {
               throw new MissingJiraAgileSupportException();
            }
            allBoards.AddRange(result.Values);
         } while (result.IsLast == false);

         return allBoards;
      }

      public async Task<RawAgileBoardConfiguration> GetBoardConfiguration(int boardId)
      {
         var client = BuildRestClient();
         var request = new RestRequest("/rest/agile/latest/board/{id}/configuration", Method.GET);
         request.AddUrlSegment("id", boardId.ToString());

         var response = await client.ExecuteTaskAsync(request);
         var result = JsonConvert.DeserializeObject<RawAgileBoardConfiguration>(response.Content);

         return result;
      }
   }
}
