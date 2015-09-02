using JiraManager.Api;

namespace JiraManager.Messages.Actions
{
   public class LogMessage
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
