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
using GalaSoft.MvvmLight.Threading;

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
         if (_connectionPropertyPane == null)
         {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
               _connectionPropertyPane = new ConnectionManager { DataContext = new ConnectionViewModel(_messageBus, _configuration) };
               ShowConnectionPropertyPane();
            });
            return;
         }
         _messageBus.Send(new ShowPropertyPaneMessage(this, "JIRA", _connectionPropertyPane, false));
      }

      private void ShowSearchPropertyPane()
      {
         if (_searchPropertyPane == null)
         {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
               _searchPropertyPane = new SearchIssues { DataContext = new SearchIssuesViewModel(_messageBus) };
               ShowConnectionPropertyPane();
            });
            return;
         }
         _messageBus.Send(new ShowPropertyPaneMessage(this, "search", _searchPropertyPane, false));
      }

      private void ShowIssuesListPane()
      {
         if (_issueListDocumentPane == null)
         {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
               _issueListDocumentPane = new IssueListDisplay { DataContext = new IssueListViewModel(_messageBus) };
               ShowConnectionPropertyPane();
            });
            return;
         }
         _messageBus.Send(new ShowPropertyPaneMessage(this, "issues", _issueListDocumentPane, false));
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
      }

      private UserControl _connectionPropertyPane;
      private UserControl _searchPropertyPane;
      private UserControl _issueListDocumentPane;
      private readonly IConfiguration _configuration;
   }
}
