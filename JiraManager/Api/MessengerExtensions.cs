using GalaSoft.MvvmLight.Messaging;
using Yakuza.JiraClient.Messages.Actions;

namespace Yakuza.JiraClient.Api
{
   internal static class MessengerExtensions
   {
      internal static void LogMessage(this IMessenger @this, string message, LogLevel level = LogLevel.Debug)
      {
         @this.Send(new LogMessage(message, level));
      }
   }
}
