using LightShell.Api.Messages.Actions;
using LightShell.Messaging.Api;

namespace LightShell.Api
{
   public static class MessengerExtensions
   {
      public static void LogMessage(this IMessageBus @this, string message, LogLevel level = LogLevel.Debug)
      {
         @this.Send(new LogMessage(message, level));
      }

      public static void LogMessage(this IMessageBus @this, LogLevel level, string messageFormat, params object[] args)
      {
         @this.Send(new LogMessage(string.Format(messageFormat, args), level));
      }
   }
}
