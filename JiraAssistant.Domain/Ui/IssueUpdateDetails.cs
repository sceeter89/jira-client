using JiraAssistant.Domain.Jira;
using System;
using System.Collections.Generic;

namespace JiraAssistant.Domain.Ui
{
    public class IssueUpdateDetails
    {
        public DateTime ChangeTime { get; set; }
        public string Author { get; set; }
        public JiraIssue Issue { get; set; }
        public IEnumerable<FieldChange> ChangeSummary { get; set; }
    }
}