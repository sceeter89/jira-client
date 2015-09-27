using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using System.Threading.Tasks;
using Yakuza.JiraClient.Api;
using Yakuza.JiraClient.Api.Messages.IO.Jira;
using Yakuza.JiraClient.Api.Model;
using Yakuza.JiraClient.Messaging.Api;

namespace Yakuza.JiraClient.IO.Jira
{
   public class MetadataRetriever : RestMicroserviceBase,
      IMicroservice,
      IHandleMessage<GetFieldsDescriptionsMessage>,
      IHandleMessage<GetIssueTypesMessage>,
      IHandleMessage<GetPrioritiesMessage>,
      IHandleMessage<GetProjectsMessage>,
      IHandleMessage<GetResolutionsMessage>,
      IHandleMessage<GetStatusesMessage>
   {
      public MetadataRetriever(IConfiguration configuration, IMessageBus messageBus)
         : base(configuration, messageBus)
      {
         _messageBus.Register(this);
      }

      public async void Handle(GetProjectsMessage message)
      {
         var projects = await GetResourceList<RawProjectInfo>("project");
         _messageBus.Send(new GetProjectsResponse(projects));
      }

      public async void Handle(GetIssueTypesMessage message)
      {
         var issueTypes = await GetResourceList<RawIssueType>("issuetype");
         _messageBus.Send(new GetIssueTypesResponse(issueTypes));
      }

      public async void Handle(GetFieldsDescriptionsMessage message)
      {
         var fields = await GetResourceList<RawFieldDefinition>("field");
         _messageBus.Send(new GetFieldsDescriptionsResponse(fields));
      }

      public async void Handle(GetPrioritiesMessage message)
      {
         var priorities = await GetResourceList<RawPriority>("priority");
         _messageBus.Send(new GetPrioritiesResponse(priorities));
      }

      public async void Handle(GetResolutionsMessage message)
      {
         var resolutions = await GetResourceList<RawResolution>("resolution");
         _messageBus.Send(new GetResolutionsResponse(resolutions));
      }

      public async void Handle(GetStatusesMessage message)
      {
         var statuses = await GetResourceList<RawStatus>("status");
         _messageBus.Send(new GetStatusesResponse(statuses));
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
