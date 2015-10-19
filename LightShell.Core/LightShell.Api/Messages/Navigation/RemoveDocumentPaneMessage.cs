using LightShell.Messaging.Api;

namespace LightShell.Api.Messages.Navigation
{
   public class RemoveDocumentPaneMessage : IMessage
   {
      public RemoveDocumentPaneMessage(object sender, string title)
      {
         Sender = sender;
         Title = title;
      }

      public object Sender { get; private set; }
      public string Title { get; private set; }
   }
}
