using Yakuza.JiraClient.Messaging.Api;

namespace Yakuza.JiraClient.InternalMessages.UI
{
   internal class UpdateUiBootstrapMessage : IMessage
   {
      public UpdateUiBootstrapMessage(string message)
      {
         Message = message;
      }

      public string Message { get; private set; }
   }
}
