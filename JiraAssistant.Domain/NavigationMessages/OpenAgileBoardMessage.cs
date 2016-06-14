using JiraAssistant.Domain.Jira;

namespace JiraAssistant.Domain.NavigationMessages
{
   public class OpenAgileBoardMessage
   {
      public OpenAgileBoardMessage(RawAgileBoard board)
      {
         Board = board;
      }

      public RawAgileBoard Board { get; private set; }
   }
}
