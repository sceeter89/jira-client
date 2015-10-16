using Yakuza.JiraClient.Messaging.Api;

namespace Yakuza.JiraClient.Api.Messages.Actions.Authentication
{
   public class GetConnectionStatusMessage : IMessage
   {
   }
   public class GetConnectionStatusResponse : IMessage
   {
      public GetConnectionStatusResponse(ConnectionStatus status)
      {
         Status = status;
      }
   }
}
