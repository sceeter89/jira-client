using JiraManager.Model;
using JiraManager.Api;
using System.Threading.Tasks;
using RestSharp;
using System.Net;

namespace JiraManager.Service
{
   public class JiraOperations : IJiraOperations
   {
      private readonly Configuration _configuration;
      private readonly RestRequest _getSessionInfoRequest = new RestRequest("/rest/auth/1/session", Method.GET);
      private readonly RestRequest _postSessionInfoRequest = new RestRequest("/rest/auth/1/session", Method.POST);
      private readonly RestRequest _deleteSessionInfoRequest = new RestRequest("/rest/auth/1/session", Method.DELETE);

      public JiraOperations(Configuration configuration)
      {
         _configuration = configuration;
      }

      public async Task<SessionCheckResponse> CheckSession()
      {
         var client = new RestClient(_configuration.JiraUrl);

         var response = await client.ExecuteGetTaskAsync<SessionInfo>(_getSessionInfoRequest);

         if (response.StatusCode == HttpStatusCode.Unauthorized)
            return new SessionCheckResponse { IsLoggedIn = false };

         return new SessionCheckResponse
         {
            IsLoggedIn = true,
            SessionInfo = response.Data
         };
      }
   }
}