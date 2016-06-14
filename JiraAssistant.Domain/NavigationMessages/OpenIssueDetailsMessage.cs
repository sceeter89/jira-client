using JiraAssistant.Domain.Jira;

namespace JiraAssistant.Domain.NavigationMessages
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
