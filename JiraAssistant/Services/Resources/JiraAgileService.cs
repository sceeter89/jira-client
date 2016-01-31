using JiraAssistant.Model;
using JiraAssistant.Model.Exceptions;
using JiraAssistant.Model.Jira;
using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JiraAssistant.Services.Resources
{
   public class JiraAgileService : BaseRestService
   {
      public JiraAgileService(AssistantConfiguration configuration)
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
            allSprints.AddRange(result.Values);
         } while (result.IsLast == false);

         return allSprints;
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
   }
}
