using GalaSoft.MvvmLight;
using Yakuza.JiraClient.Api.Messages.Actions.Authentication;
using Yakuza.JiraClient.Messaging.Api;
using System.Linq;
using GalaSoft.MvvmLight.Command;
using Yakuza.JiraClient.Api.Messages.Actions;
using Yakuza.JiraClient.Api;
using Yakuza.JiraClient.Api.Messages.IO.Exports;
using Yakuza.JiraClient.Api.Messages.Navigation;
using Yakuza.JiraClient.Controls;
using System.Reflection;

namespace Yakuza.JiraClient.ViewModel
{
   public class MenuBarViewModel : ViewModelBase,
      IHandleMessage<LoggedInMessage>,
      IHandleMessage<LoggedOutMessage>,
      IHandleMessage<FilteredIssuesListMessage>,
      IHandleMessage<NoUpdatesAvailable>,
      IHandleMessage<NewVersionAvailable>
   {
      private const string WebSiteAddress = "https://github.com/sceeter89/jira-client";
      private const string ReportIssueSiteAddress = "https://github.com/sceeter89/jira-client/issues/new";
      private bool _isLoggedIn;
      private readonly IMessageBus _messageBus;

      public RelayCommand SaveXpsCommand { get; private set; }
      public RelayCommand ShowPivotViewCommand { get; private set; }
      public RelayCommand SaveLogCommand { get; private set; }
      public RelayCommand OpenWebsiteCommand { get; private set; }
      public RelayCommand ReportIssueCommand { get; private set; }
      public RelayCommand CheckForUpdatesCommand { get; private set; }

      public MenuBarViewModel(IMessageBus messageBus)
      {
         _messageBus = messageBus;
         SaveXpsCommand = new RelayCommand(() => _messageBus.Send(new GetFilteredIssuesListMessage()), () => _isLoggedIn);
         ShowPivotViewCommand = new RelayCommand(() => _messageBus.Send(new ShowDocumentPaneMessage
         (
            this, "pivot", new PivotReportingGrid(), new PivotReportingProperties()
            )), () => _isLoggedIn);
         SaveLogCommand = new RelayCommand(() => _messageBus.Send(new SaveLogOutputToFileMessage()));
         OpenWebsiteCommand = new RelayCommand(() => System.Diagnostics.Process.Start(WebSiteAddress));
         ReportIssueCommand = new RelayCommand(() => System.Diagnostics.Process.Start(ReportIssueSiteAddress));
         CheckForUpdatesCommand = new RelayCommand(() => _messageBus.Send(new CheckForUpdatesMessage(Assembly.GetEntryAssembly().GetName().Version)));

         _messageBus.Register(this);
      }

      public void Handle(LoggedOutMessage message)
      {
         _isLoggedIn = false;
         RefreshCommands();
      }

      public void Handle(LoggedInMessage message)
      {
         _isLoggedIn = true;
         RefreshCommands();
      }

      private void RefreshCommands()
      {
         foreach (var command in GetType().GetProperties().Where(p => p.PropertyType == typeof(RelayCommand)))
         {
            ((RelayCommand)command.GetValue(this)).RaiseCanExecuteChanged();
         }
      }

      public void Handle(FilteredIssuesListMessage message)
      {
         if (message.FilteredIssues == null || message.FilteredIssues.Any() == false)
         {
            _messageBus.LogMessage("No issues to export.", LogLevel.Warning);
            return;
         }

         _messageBus.Send(new GenerateScrumCardsMessage(message.FilteredIssues));
      }

      public void Handle(NoUpdatesAvailable message)
      {
         _messageBus.LogMessage("You are using latest version available.", LogLevel.Info);
      }

      public void Handle(NewVersionAvailable message)
      {
         _messageBus.LogMessage("New version is available. Visit website for download: " + WebSiteAddress, LogLevel.Info);
      }
   }
}
