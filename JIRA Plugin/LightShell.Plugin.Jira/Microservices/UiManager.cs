using System;
using LightShell.Api;
using LightShell.Messaging.Api;
using LightShell.Plugin.Jira.Api.Messages.Actions.Authentication;
using LightShell.Plugin.Jira.Api.Messages.Action;
using Telerik.Windows.Controls;
using LightShell.Plugin.Jira.Controls;
using LightShell.Api.Messages.Navigation;
using System.Windows.Controls;
using LightShell.Plugin.Jira.Api;

namespace LightShell.Plugin.Jira.Microservices
{
   public class UiManager : IMicroservice,
      IHandleMessage<LoggedInMessage>,
      IHandleMessage<LoggedOutMessage>,
      IHandleMessage<OpenConnectionTabMessage>
   {
      private IMessageBus _messageBus;

      public UiManager(IConfiguration configuration)
      {
         _configuration = configuration;
      }

      public void Handle(OpenConnectionTabMessage message)
      {
         ShowConnectionPropertyPane();
      }

      private void ShowConnectionPropertyPane()
      {
         _messageBus.Send(new ShowPropertyPaneMessage(this, "JIRA", ConnectionPropertyPane, false));
      }

      private void ShowSearchPropertyPane()
      {
         _messageBus.Send(new ShowPropertyPaneMessage(this, "search", SearchPropertyPane, false));
      }

      private void ShowIssuesListPane()
      {
         _messageBus.Send(new ShowPropertyPaneMessage(this, "issues", IssueListDocumentPane, false));
      }

      public void Handle(LoggedOutMessage message)
      {
         _messageBus.Send(new ClearDocumentPanesMessage());
         _messageBus.Send(new ClearPropertyPanesMessage());

         ShowConnectionPropertyPane();
      }

      public void Handle(LoggedInMessage message)
      {
         _messageBus.Send(new ClearDocumentPanesMessage());
         _messageBus.Send(new ClearPropertyPanesMessage());

         ShowIssuesListPane();
         ShowConnectionPropertyPane();
         ShowSearchPropertyPane();
      }

      public void Initialize(IMessageBus messageBus)
      {
         _messageBus = messageBus;
         ShowConnectionPropertyPane();
      }

      private UserControl _connectionPropertyPane;
      private UserControl _searchPropertyPane;
      private UserControl _issueListDocumentPane;
      private readonly IConfiguration _configuration;

      private UserControl SearchPropertyPane
      {
         get
         {
            if (_searchPropertyPane == null)
               _searchPropertyPane = new SearchIssues { DataContext = new SearchIssuesViewModel(_messageBus) };

            return _searchPropertyPane;
         }
      }
      private UserControl ConnectionPropertyPane
      {
         get
         {
            if (_connectionPropertyPane == null)
               _connectionPropertyPane = new ConnectionManager { DataContext = new ConnectionViewModel(_messageBus, _configuration) };

            return _connectionPropertyPane;
         }
      }

      private UserControl IssueListDocumentPane
      {
         get
         {
            if (_issueListDocumentPane == null)
               _issueListDocumentPane = new IssueListDisplay { DataContext = new IssueListViewModel(_messageBus) };

            return _issueListDocumentPane;
         }
      }

   }
}
