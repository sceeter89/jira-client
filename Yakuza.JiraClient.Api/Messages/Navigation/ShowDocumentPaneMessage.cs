using System.Windows.Controls;
using Yakuza.JiraClient.Messaging.Api;

namespace Yakuza.JiraClient.Api.Messages.Navigation
{
   public class ShowDocumentPaneMessage : IMessage
   {
      public ShowDocumentPaneMessage(object sender, string title, UserControl paneContent)
      {
         Sender=  sender;
         Title = title;
         PaneContent = paneContent;
      }

      public UserControl PaneContent { get; private set; }
      public object Sender { get; private set; }
      public string Title { get; private set; }
   }
}
