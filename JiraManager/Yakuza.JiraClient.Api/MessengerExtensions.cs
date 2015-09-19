using GalaSoft.MvvmLight.Messaging;
using Yakuza.JiraClient.Api.Messages.Actions;

namespace Yakuza.JiraClient.Api
{
   public static class MessengerExtensions
   {
      public static void LogMessage(this IMessenger @this, string message, LogLevel level = LogLevel.Debug)
      {
         @this.Send(new LogMessage(message, level));
      }
   }
}
