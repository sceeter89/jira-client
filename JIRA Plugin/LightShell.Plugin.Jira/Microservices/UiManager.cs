using System;
using LightShell.Api;
using LightShell.Messaging.Api;
using LightShell.Plugin.Jira.Api.Messages.Actions.Authentication;
using LightShell.Plugin.Jira.Api.Messages.Action;

namespace LightShell.Plugin.Jira.Microservices
{
   public class UiManager : IMicroservice,
      IHandleMessage<LoggedInMessage>,
      IHandleMessage<LoggedOutMessage>,
      IHandleMessage<OpenConnectionTabMessage>
   {
      public void Handle(OpenConnectionTabMessage message)
      {
         throw new NotImplementedException();
      }

      public void Handle(LoggedOutMessage message)
      {
         DocumentPanes.Clear();
         PropertyPanes.Clear();
         PropertyPanes.Add(ConnectionPropertyPane);
         PropertyPanes.Add(CustomPropertyPane);
      }

      public void Handle(LoggedInMessage message)
      {
         DocumentPanes.Clear();
         DocumentPanes.Add(IssueListDocumentPane);
         FocusDocumentPane(IssueListDocumentPane);

         PropertyPanes.Clear();
         PropertyPanes.Add(SearchPropertyPane);
         PropertyPanes.Add(ConnectionPropertyPane);
         PropertyPanes.Add(CustomPropertyPane);
         FocusPropertyPane(SearchPropertyPane);
      }

      public void Initialize(IMessageBus messageBus)
      {
         FocusPropertyPane(ConnectionPropertyPane);
      }
   }
}
