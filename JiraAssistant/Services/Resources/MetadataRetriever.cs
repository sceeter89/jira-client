using JiraAssistant.Model.Jira;
using JiraAssistant.Services.Settings;
using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JiraAssistant.Services.Resources
{
   public class MetadataRetriever : BaseRestService
   {
      public MetadataRetriever(AssistantSettings configuration)
         : base(configuration)
      {
      }

      public async Task<IEnumerable<RawProjectInfo>> GetProjects()
      {
         return await GetResourceList<RawProjectInfo>("project");
      }

      public async Task<IEnumerable<RawIssueType>> GetIssueTypes()
      {
         return await GetResourceList<RawIssueType>("issuetype");
      }

      public async Task<IEnumerable<RawFieldDefinition>> GetFieldsDefinitions()
      {
         return await GetResourceList<RawFieldDefinition>("field");
      }

      public async Task<IEnumerable<RawPriority>> GetPriorities()
      {
         return await GetResourceList<RawPriority>("priority");
      }

      public async Task<IEnumerable<RawResolution>> GetResolutions()
      {
         return await GetResourceList<RawResolution>("resolution");
      }

      public async Task<IEnumerable<RawStatus>> GetStatuses()
      {
         return await GetResourceList<RawStatus>("status");
      }

      public async Task<RawFilterDefinition> GetFilterDefinition(int filterId)
      {
         var client = BuildRestClient();
         var request = new RestRequest("/rest/api/latest/filter/{id}", Method.GET);
         request.AddUrlSegment("id", filterId.ToString());

         var response = await client.ExecuteTaskAsync(request);
         return JsonConvert.DeserializeObject<RawFilterDefinition>(response.Content);
      }

      private async Task<IEnumerable<T>> GetResourceList<T>(string resourceName)
      {
         var client = BuildRestClient();
         var request = new RestRequest("/rest/api/latest/" + resourceName, Method.GET);

         var response = await client.ExecuteTaskAsync(request);
         var result = await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<List<T>>(response.Content));

         return result;
      }
   }
}
