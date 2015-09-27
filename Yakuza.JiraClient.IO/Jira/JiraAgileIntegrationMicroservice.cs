using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using Yakuza.JiraClient.Api;
using Yakuza.JiraClient.Api.Messages.IO.Jira;
using Yakuza.JiraClient.Api.Model;
using Yakuza.JiraClient.Messaging.Api;

namespace Yakuza.JiraClient.IO.Jira
{
   public class JiraAgileIntegrationMicroservice : RestMicroserviceBase,
      IMicroService,
      IHandleMessage<GetAgileBoardsResponse>,
      IHandleMessage<GetAgileSprintsMessage>
   {
      public JiraAgileIntegrationMicroservice(IConfiguration configuration, IMessageBus messageBus)
         : base(configuration, messageBus)
      {

      }

      public async void Handle(GetAgileSprintsMessage message)
      {
         var client = BuildRestClient();
         var request = new RestRequest("/rest/agile/latest/board/{id}/sprint", Method.GET);
         request.AddQueryParameter("maxResults", "500");
         request.AddQueryParameter("startAt", "0");
         request.AddUrlSegment("id", message.Board.Id.ToString());

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

         _messageBus.Send(new GetAgileSprintsResponse(message.Board, allSprints));
      }

      public async void Handle(GetAgileBoardsResponse message)
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
            allBoards.AddRange(result.Values);
         } while (result.IsLast == false);

         _messageBus.Send(new GetAgileBoardsResponse(allBoards));
      }
   }
}
