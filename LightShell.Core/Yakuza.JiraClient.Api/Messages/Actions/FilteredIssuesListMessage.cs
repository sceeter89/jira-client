using System.Collections.Generic;
using Yakuza.JiraClient.Api.Model;
using Yakuza.JiraClient.Messaging.Api;

namespace Yakuza.JiraClient.Api.Messages.Actions
{
   public class FilteredIssuesListMessage : IMessage
   {
      public FilteredIssuesListMessage(IEnumerable<JiraIssue> issues)
      {
         FilteredIssues = issues;
      }

      public IEnumerable<JiraIssue> FilteredIssues { get; private set; }
   }
}
