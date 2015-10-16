using LightShell.Messaging.Api;

namespace LightShell.Api.Messages.Navigation
{
   public class RemovePropertyPaneMessage : IMessage
   {
      public RemovePropertyPaneMessage(object sender, string title)
      {
         Sender = sender;
         Title = title;
      }

      public object Sender { get; private set; }
      public string Title { get; private set; }
   }
}
