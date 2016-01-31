using RestSharp;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using System;
using JiraAssistant.Model;
using System.Threading.Tasks;
using JiraAssistant.Model.Jira;
using JiraAssistant.Model.Exceptions;

namespace JiraAssistant.Services.Resources
{
   public class JiraSessionService : BaseRestService
   {
      public JiraSessionService(AssistantConfiguration configuration)
         : base(configuration)
      {
      }

      public async Task<RawProfileDetails> GetProfileDetails()
      {
         var client = BuildRestClient();
         var request = new RestRequest("/rest/api/latest/myself", Method.GET);

         var response = await client.ExecuteTaskAsync(request);
         var result = JsonConvert.DeserializeObject<RawProfileDetails>(response.Content);

         return result;
      }

      public async void Logout()
      {
         var client = BuildRestClient();

         var response = await client.ExecuteTaskAsync(new RestRequest("/rest/auth/1/session", Method.DELETE));
         Configuration.JiraSessionId = "";
      }

      public async Task<bool> CheckJiraSession()
      {
         try
         {
            var client = BuildRestClient();

            var response = await client.ExecuteGetTaskAsync<RawSessionInfo>(new RestRequest("/rest/auth/1/session"));

            if (response.StatusCode == HttpStatusCode.Unauthorized || response.Data == null)
            {
               Configuration.JiraSessionId = "";
               return false;
            }

            return true;
         }
         catch (UriFormatException)
         {
            return false;
         }
      }

      public async Task AttemptLogin(string jiraUrl, string username, string password)
      {
         try
         {
            Configuration.JiraUrl = jiraUrl;
            Configuration.Username = username;
            var client = BuildRestClient();

            var sessionInfoRequest = new RestRequest("/rest/auth/1/session");
            sessionInfoRequest.RequestFormat = DataFormat.Json;
            sessionInfoRequest.AddJsonBody(new Dictionary<string, string>
            {
               {"username", username },
               {"password", password }
            });
            var response = await client.ExecutePostTaskAsync<RawSuccessfulLoginParameters>(sessionInfoRequest);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
               throw new LoginFailedException("Invalid username or password");
            }

            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
               throw new LoginFailedException("User was not allowed to log in. Try to login via browser");
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
               throw new LoginFailedException("Server returned unexpected response code: " + response.StatusCode);
            }

            if (response.Data == null)
            {
               throw new LoginFailedException(string.Format("Given address '{0}' does not point at valid JIRA server.", jiraUrl));
            }

            Configuration.JiraSessionId = response.Data.Session.Value;
         }
         catch (UriFormatException)
         {
            throw new LoginFailedException(string.Format("Invalid JIRA server address."));
         }
      }
   }
}
