using JiraAssistant.Model.Jira;

namespace JiraAssistant.Model.NavigationMessages
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
