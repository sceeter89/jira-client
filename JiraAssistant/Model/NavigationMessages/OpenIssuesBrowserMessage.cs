using JiraAssistant.Model.Jira;
using System.Collections.Generic;

namespace JiraAssistant.Model.NavigationMessages
{
   public class OpenIssuesBrowserMessage
   {
      public OpenIssuesBrowserMessage(IList<JiraIssue> issues)
      {
         Issues = issues;
      }
      
      public IList<JiraIssue> Issues { get; private set; }
   }
}
