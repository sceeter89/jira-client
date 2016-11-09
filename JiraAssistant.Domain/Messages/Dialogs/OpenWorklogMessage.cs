using JiraAssistant.Domain.Jira;
using System.Collections.Generic;

namespace JiraAssistant.Domain.Messages.Dialogs
{
    public class OpenWorklogMessage
    {
        public OpenWorklogMessage(IEnumerable<JiraIssue> issues)
        {
            Issues = issues;
        }
        
        public IEnumerable<JiraIssue> Issues { get; private set; }
    }
}
