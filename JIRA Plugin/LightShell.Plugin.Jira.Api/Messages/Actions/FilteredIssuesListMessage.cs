using System.Collections.Generic;
using LightShell.Plugin.Jira.Api.Model;
using LightShell.Messaging.Api;

namespace LightShell.Plugin.Jira.Api.Messages.Actions
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
