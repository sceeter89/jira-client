using JiraAssistant.Model.Jira;

namespace JiraAssistant.Model.NavigationMessages
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
