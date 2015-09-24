using Yakuza.JiraClient.Messaging.Api;

namespace Yakuza.JiraClient.Api.Messages.Actions
{
   public class LogMessage : IMessage
   {
      public LogMessage(string message, LogLevel level = LogLevel.Debug)
      {
         Message = message;
         Level = level;
      }

      public LogLevel Level { get; private set; }
      public string Message { get; private set; }
   }
}
