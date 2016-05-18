using JiraAssistant.Model.Jira;

namespace JiraAssistant.Model.NavigationMessages
{
   public class OpenIssueDetailsMessage
   {
      public OpenIssueDetailsMessage(JiraIssue issue)
      {
         Issue = issue;
      }

      public JiraIssue Issue { get; private set; }
   }
}
