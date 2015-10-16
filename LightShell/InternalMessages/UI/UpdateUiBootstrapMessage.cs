using LightShell.Messaging.Api;

namespace LightShell.InternalMessages.UI
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
