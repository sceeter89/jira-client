using Yakuza.JiraClient.Api.Model;
using Yakuza.JiraClient.Messaging.Api;

namespace Yakuza.JiraClient.Api.Messages.IO.Jira
{
   public class CheckJiraSessionMessage : IMessage
   {
   }

   public class CheckJiraSessionResponse : IMessage
   {
      public CheckJiraSessionResponse(SessionCheckResponse response)
      {
         Response = response;
      }

      public SessionCheckResponse Response { get; private set; }
   }
}
