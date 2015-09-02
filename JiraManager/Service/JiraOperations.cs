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
      private readonly RestRequest _sessionInfoRequest = new RestRequest("/rest/auth/1/session");
      private readonly RestRequest _logoutRequest = new RestRequest("/rest/auth/1/session", Method.DELETE);

      public JiraOperations(Configuration configuration)
      {
         _configuration = configuration;
      }

      public async Task<SessionCheckResponse> CheckSession()
      {
         if(IsConfigValid() == false)
            return new SessionCheckResponse { IsLoggedIn = false };

         var client = new RestClient(_configuration.JiraUrl);

         var response = await client.ExecuteGetTaskAsync<SessionInfo>(_sessionInfoRequest);

         if (response.StatusCode == HttpStatusCode.Unauthorized)
            return new SessionCheckResponse { IsLoggedIn = false };

         return new SessionCheckResponse
         {
            IsLoggedIn = true,
            SessionInfo = response.Data
         };
      }

      public async Task<LoginAttemptResult> TryToLogin(string username, string password)
      {
         if (IsConfigValid() == false)
            return new LoginAttemptResult { WasSuccessful = false, ErrorMessage = "Jira address is invalid" };

         var client = new RestClient(_configuration.JiraUrl);

         var response = await client.ExecutePostTaskAsync<SuccessfulLoginParameters>(_sessionInfoRequest);

         if (response.StatusCode == HttpStatusCode.Unauthorized)
            return new LoginAttemptResult { WasSuccessful = false, ErrorMessage = "Invalid username or password" };

         if (response.StatusCode == HttpStatusCode.Forbidden)
            return new LoginAttemptResult { WasSuccessful = false, ErrorMessage = "User was not allowed to log in. Try to login via browser" };

         _configuration.JiraSessionId = response.Data.Session.Value;
         return new LoginAttemptResult { WasSuccessful = true };
      }

      public async Task Logout()
      {
         var client = new RestClient(_configuration.JiraUrl);

         var response = await client.ExecuteTaskAsync(_logoutRequest);
      }

      private bool IsConfigValid()
      {
         return string.IsNullOrWhiteSpace(_configuration.JiraSessionId) == false;
      }
   }
}