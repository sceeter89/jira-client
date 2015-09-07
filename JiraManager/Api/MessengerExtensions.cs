using GalaSoft.MvvmLight.Messaging;
using JiraManager.Messages.Actions;

namespace JiraManager.Api
{
   internal static class MessengerExtensions
   {
      internal static void LogMessage(this IMessenger @this, string message, LogLevel level = LogLevel.Debug)
      {
         @this.Send(new LogMessage(message, level));
      }
   }
}
