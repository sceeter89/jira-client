using RestSharp;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using LightShell.Messaging.Api;
using LightShell.Plugin.Jira.Api.Messages.IO.Jira;
using LightShell.Plugin.Jira.Api.Model;
using LightShell.Plugin.Jira.Api.Messages.Actions.Authentication;
using LightShell.Plugin.Jira.Api;

namespace LightShell.Plugin.Jira.Microservices
{
   public class SessionInteractionMicroservice : RestMicroserviceBase,
      IHandleMessage<AttemptLoginMessage>,
      IHandleMessage<CheckJiraSessionMessage>,
      IHandleMessage<LogoutMessage>,
      IHandleMessage<GetProfileDetailsMessage>
   {
      public SessionInteractionMicroservice(IConfiguration configuration)
         : base(configuration)
      {
      }

      public async void Handle(GetProfileDetailsMessage message)
      {
         var client = BuildRestClient();
         var request = new RestRequest("/rest/api/latest/myself", Method.GET);

         var response = await client.ExecuteTaskAsync(request);
         var result = JsonConvert.DeserializeObject<RawProfileDetails>(response.Content);

         _messageBus.Send(new GetProfileDetailsResponse(result));
      }

      public async void Handle(LogoutMessage message)
      {
         var client = BuildRestClient();

         var response = await client.ExecuteTaskAsync(new RestRequest("/rest/auth/1/session", Method.DELETE));
         _configuration.JiraSessionId = "";
         _messageBus.Send(new LoggedOutMessage());
      }

      public async void Handle(CheckJiraSessionMessage message)
      {
         if (IsConfigValid() == false)
         {
            _messageBus.Send(new CheckJiraSessionResponse(new SessionCheckResponse { IsLoggedIn = false }));
            return;
         }

         var client = BuildRestClient();

         var response = await client.ExecuteGetTaskAsync<RawSessionInfo>(new RestRequest("/rest/auth/1/session"));

         if (response.StatusCode == HttpStatusCode.Unauthorized || response.Data == null)
         {
            _configuration.JiraSessionId = "";
            _messageBus.Send(new CheckJiraSessionResponse(new SessionCheckResponse { IsLoggedIn = false }));
            return;
         }

         _messageBus.Send(new CheckJiraSessionResponse(new SessionCheckResponse
         {
            IsLoggedIn = true,
            SessionInfo = response.Data
         }));
      }

      public async void Handle(AttemptLoginMessage message)
      {
         if (IsConfigValid() == false)
         {
            _messageBus.Send(new AttemptLoginResponse(new LoginAttemptResult { WasSuccessful = false, ErrorMessage = "Jira address is invalid" }));
            return;
         }

         var client = BuildRestClient();

         var sessionInfoRequest = new RestRequest("/rest/auth/1/session");
         sessionInfoRequest.RequestFormat = DataFormat.Json;
         sessionInfoRequest.AddJsonBody(new Dictionary<string, string>
         {
            {"username", message.Username },
            {"password", message.Password }
         });
         var response = await client.ExecutePostTaskAsync<RawSuccessfulLoginParameters>(sessionInfoRequest);

         if (response.StatusCode == HttpStatusCode.Unauthorized)
         {
            _messageBus.Send(new AttemptLoginResponse(new LoginAttemptResult { WasSuccessful = false, ErrorMessage = "Invalid username or password" }));
            return;
         }

         if (response.StatusCode == HttpStatusCode.Forbidden)
         {
            _messageBus.Send(new AttemptLoginResponse(new LoginAttemptResult { WasSuccessful = false, ErrorMessage = "User was not allowed to log in. Try to login via browser" }));
            return;
         }

         if (response.StatusCode != HttpStatusCode.OK)
         {
            _messageBus.Send(new AttemptLoginResponse(new LoginAttemptResult { WasSuccessful = false, ErrorMessage = "Server returned unexpected response code: " + response.StatusCode }));
            return;
         }

         if (response.Data == null)
         {
            _messageBus.Send(new AttemptLoginResponse(new LoginAttemptResult { WasSuccessful = false, ErrorMessage = string.Format("Given address '{0}' does not point at valid JIRA server.", _configuration.JiraUrl) }));
            return;
         }

         _configuration.JiraSessionId = response.Data.Session.Value;
         _messageBus.Send(new AttemptLoginResponse(new LoginAttemptResult { WasSuccessful = true }));
      }
   }
}
