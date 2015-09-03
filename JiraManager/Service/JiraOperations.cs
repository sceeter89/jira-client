using JiraManager.Model;
using JiraManager.Api;
using System.Threading.Tasks;
using RestSharp;
using System.Net;
using System.Collections.Generic;

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
         if (IsConfigValid() == false)
            return new SessionCheckResponse { IsLoggedIn = false };

         var client = BuildRestClient();

         var response = await client.ExecuteGetTaskAsync<SessionInfo>(_sessionInfoRequest);

         if (response.StatusCode == HttpStatusCode.Unauthorized)
         {
            _configuration.JiraSessionId = "";
            return new SessionCheckResponse { IsLoggedIn = false };
         }

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

         var client = BuildRestClient();

         var sessionInfoRequest = new RestRequest("/rest/auth/1/session");
         sessionInfoRequest.RequestFormat = DataFormat.Json;
         sessionInfoRequest.AddJsonBody(new Dictionary<string, string>
         {
            {"username", username },
            {"password", password }
         });
         var response = await client.ExecutePostTaskAsync<SuccessfulLoginParameters>(sessionInfoRequest);

         if (response.StatusCode == HttpStatusCode.Unauthorized)
            return new LoginAttemptResult { WasSuccessful = false, ErrorMessage = "Invalid username or password" };

         if (response.StatusCode == HttpStatusCode.Forbidden)
            return new LoginAttemptResult { WasSuccessful = false, ErrorMessage = "User was not allowed to log in. Try to login via browser" };

         if (response.StatusCode != HttpStatusCode.OK)
            return new LoginAttemptResult { WasSuccessful = false, ErrorMessage = "Server returned unexpected response code: " + response.StatusCode };

         _configuration.JiraSessionId = response.Data.Session.Value;
         return new LoginAttemptResult { WasSuccessful = true };
      }

      public async Task Logout()
      {
         var client = BuildRestClient();

         var response = await client.ExecuteTaskAsync(_logoutRequest);
         _configuration.JiraSessionId = "";
      }

      private RestClient BuildRestClient()
      {
         var client = new RestClient(_configuration.JiraUrl);
         client.AddDefaultHeader("Content-Type", "Application/json");
         if (string.IsNullOrEmpty(_configuration.JiraSessionId) == false)
         { 
            client.CookieContainer = new CookieContainer();
            client.CookieContainer.Add(new Cookie("JSESSIONID", _configuration.JiraSessionId, "/", client.BaseUrl.Host));
         }

         return client;
      }

      private bool IsConfigValid()
      {
         return string.IsNullOrWhiteSpace(_configuration.JiraUrl) == false;
      }
   }
}