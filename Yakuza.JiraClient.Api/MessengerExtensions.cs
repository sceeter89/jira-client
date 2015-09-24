using Yakuza.JiraClient.Api.Messages.Actions;
using Yakuza.JiraClient.Messaging.Api;

namespace Yakuza.JiraClient.Api
{
   public static class MessengerExtensions
   {
      public static void LogMessage(this IMessageBus @this, string message, LogLevel level = LogLevel.Debug)
      {
         @this.Send(new LogMessage(message, level));
      }
   }
}
