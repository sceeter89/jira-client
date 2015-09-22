using System.Collections.Generic;
using Yakuza.JiraClient.Api.Model;

namespace Yakuza.JiraClient.Api.Messages.Actions
{
   public class FilteredIssuesListMessage
   {
      public FilteredIssuesListMessage(IEnumerable<JiraIssue> issues)
      {
         FilteredIssues = issues;
      }

      public IEnumerable<JiraIssue> FilteredIssues { get; private set; }
   }
}
