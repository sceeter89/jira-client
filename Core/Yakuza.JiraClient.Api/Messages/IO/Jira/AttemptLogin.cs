using Yakuza.JiraClient.Api.Model;
using Yakuza.JiraClient.Messaging.Api;

namespace Yakuza.JiraClient.Api.Messages.IO.Jira
{
   public class AttemptLoginMessage : IMessage
   {
      public AttemptLoginMessage(string jiraUrl, string username, string password)
      {
         JiraUrl = jiraUrl;
         Username= username;
         Password= password;
      }

      public string JiraUrl { get; private set; }
      public string Password { get; private set; }
      public string Username { get; private set; }
   }

   public class AttemptLoginResponse : IMessage
   {
      public AttemptLoginResponse(LoginAttemptResult result)
      {
         Result = result;
      }

      public LoginAttemptResult Result { get; private set; }
   }
}
