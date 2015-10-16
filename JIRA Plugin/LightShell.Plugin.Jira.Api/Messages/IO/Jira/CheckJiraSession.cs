using LightShell.Plugin.Jira.Api.Model;
using LightShell.Messaging.Api;

namespace LightShell.Plugin.Jira.Api.Messages.IO.Jira
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
