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

namespace Yakuza.JiraClient.ViewModel
{
   public class MenuBarViewModel : ViewModelBase,
      IHandleMessage<LoggedInMessage>,
      IHandleMessage<LoggedOutMessage>,
      IHandleMessage<FilteredIssuesListMessage>
   {
      private bool _isLoggedIn;
      private readonly IMessageBus _messageBus;

      public RelayCommand SaveXpsCommand { get; private set; }
      public RelayCommand ShowPivotViewCommand { get; private set; }

      public MenuBarViewModel(IMessageBus messageBus)
      {
         _messageBus = messageBus;
         SaveXpsCommand = new RelayCommand(() => _messageBus.Send(new GetFilteredIssuesListMessage()), () => _isLoggedIn);
         ShowPivotViewCommand = new RelayCommand(() => _messageBus.Send(new ShowDocumentPaneMessage
         (
            this, "pivot", new PivotReportingGrid(), new PivotReportingProperties()
            )), () => _isLoggedIn);

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

   }
}
