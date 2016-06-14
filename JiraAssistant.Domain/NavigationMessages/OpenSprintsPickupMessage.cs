using JiraAssistant.Domain.Jira;

namespace JiraAssistant.Domain.NavigationMessages
{
   public class OpenSprintsPickupMessage
   {
      public OpenSprintsPickupMessage(AgileBoardIssues boardContent)
      {
         BoardContent = boardContent;
      }

      public AgileBoardIssues BoardContent { get; private set; }
   }
}
