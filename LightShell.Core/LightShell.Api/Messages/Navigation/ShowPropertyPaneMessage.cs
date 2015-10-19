using LightShell.Messaging.Api;
using System.Windows.Controls;

namespace LightShell.Api.Messages.Navigation
{
   public class ShowPropertyPaneMessage : IMessage
   {
      public ShowPropertyPaneMessage(object sender, string title, UserControl paneContent, bool isUserCloseable)
      {
         Sender = sender;
         Title = title;
         PaneContent = paneContent;
         IsUserCloseable = isUserCloseable;
      }

      public bool IsUserCloseable { get; private set; }
      public UserControl PaneContent { get; private set; }
      public object Sender { get; private set; }
      public string Title { get; private set; }
   }
}
