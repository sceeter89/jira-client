using JiraAssistant.Model.Jira;

namespace JiraAssistant.Model.NavigationMessages
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
