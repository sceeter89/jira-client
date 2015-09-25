using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using Yakuza.JiraClient.Api;
using Yakuza.JiraClient.Api.Messages.IO.Jira;
using Yakuza.JiraClient.Api.Model;
using Yakuza.JiraClient.Messaging.Api;

namespace Yakuza.JiraClient.IO.Jira
{
   public class SessionInteractionMicroservice : RestMicroserviceBase,
      IMicroService,
      IHandleMessage<AttemptLoginMessage>,
      IHandleMessage<CheckJiraSessionMessage>,
      IHandleMessage<LogoutMessage>
   {
      public SessionInteractionMicroservice(IConfiguration configuration, IMessageBus messageBus)
         : base(configuration, messageBus)
      {
         messageBus.Register(this);
      }

      public async void Handle(LogoutMessage message)
      {
         var client = BuildRestClient();

         var response = await client.ExecuteTaskAsync(new RestRequest("/rest/auth/1/session", Method.DELETE));
         _configuration.JiraSessionId = "";
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
            _messageBus.Send(new LoginAttemptResult { WasSuccessful = false, ErrorMessage = "Jira address is invalid" });
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
            _messageBus.Send(new LoginAttemptResult { WasSuccessful = false, ErrorMessage = "Invalid username or password" });
            return;
         }

         if (response.StatusCode == HttpStatusCode.Forbidden)
         {
            _messageBus.Send(new LoginAttemptResult { WasSuccessful = false, ErrorMessage = "User was not allowed to log in. Try to login via browser" });
            return;
         }

         if (response.StatusCode != HttpStatusCode.OK)
         {
            _messageBus.Send(new LoginAttemptResult { WasSuccessful = false, ErrorMessage = "Server returned unexpected response code: " + response.StatusCode });
            return;
         }

         _configuration.JiraSessionId = response.Data.Session.Value;
         _messageBus.Send(new LoginAttemptResult { WasSuccessful = true });
      }
   }
}
