using Yakuza.JiraClient.Messaging.Api;

namespace Yakuza.JiraClient.Api.Model
{
   public class LoginAttemptResult : IMessage
   {
      public bool WasSuccessful { get; set; }
      public string ErrorMessage { get; set; }
   }
}