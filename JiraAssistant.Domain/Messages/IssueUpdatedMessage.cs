using JiraAssistant.Domain.Jira;
using System;
using System.Collections.Generic;

namespace JiraAssistant.Domain.Messages
{
    public class IssueUpdatedMessage
    {
        public IssueUpdatedMessage(JiraIssue issue, IEnumerable<RawChangelogItem> changes, DateTime occurred, RawUserInfo author)
        {
            Issue = issue;
            Changes = changes;
            Occurred = occurred;
            Author = author;
        }

        public RawUserInfo Author { get; private set; }
        public IEnumerable<RawChangelogItem> Changes { get; private set; }
        public JiraIssue Issue { get; private set; }
        public DateTime Occurred { get; private set; }
    }
}