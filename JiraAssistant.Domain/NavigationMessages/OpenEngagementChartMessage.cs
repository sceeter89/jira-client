using JiraAssistant.Domain.Jira;
using System.Collections.Generic;

namespace JiraAssistant.Domain.NavigationMessages
{
   public class OpenEngagementChartMessage
   {
      public OpenEngagementChartMessage(IList<JiraIssue> issues)
      {
         Issues = issues;
      }
      
      public IList<JiraIssue> Issues { get; private set; }
   }
}
