using System.Linq;
using Yakuza.JiraClient.Api;
using Yakuza.JiraClient.Api.Messages.Actions;
using Yakuza.JiraClient.Api.Messages.IO.Exports;
using Yakuza.JiraClient.Messaging.Api;

namespace Yakuza.JiraClient.Plugins.Agile
{
   public class CardsPrintingHandler : IMicroservice,
      IHandleMessage<FilteredIssuesListMessage>
   {
      private IMessageBus _messageBus;
      private bool _pendingGenerationRequest = false;

      public void Handle(FilteredIssuesListMessage message)
      {
         if (_pendingGenerationRequest == false)
            return;

         if (message.FilteredIssues == null || message.FilteredIssues.Any() == false)
         {
            _messageBus.LogMessage("No issues to export.", LogLevel.Warning);
            return;
         }
         _pendingGenerationRequest = false;
         _messageBus.Send(new GenerateScrumCardsMessage(message.FilteredIssues));
      }

      public void Initialize(IMessageBus messageBus)
      {
         _messageBus = messageBus;

         _messageBus.Register(this);
      }

      internal void PrintCards()
      {
         _pendingGenerationRequest = true;
         _messageBus.Send(new GetFilteredIssuesListMessage());
      }
   }
}
