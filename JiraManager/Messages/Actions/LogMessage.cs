namespace JiraManager.Messages.Actions
{
   public class LogMessage
   {
      public LogMessage(string message)
      {
         Message = message;
      }

      public string Message { get; private set; }
   }
}
