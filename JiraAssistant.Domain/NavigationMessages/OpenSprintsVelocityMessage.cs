using JiraAssistant.Domain.Jira;

namespace JiraAssistant.Domain.NavigationMessages
{
   public class OpenSprintsVelocityMessage
   {
      public OpenSprintsVelocityMessage(AgileBoardIssues issues)
      {
         Issues = issues;
      }
      
      public AgileBoardIssues Issues { get; private set; }
   }
}
