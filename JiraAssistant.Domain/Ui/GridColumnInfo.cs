using JiraAssistant.Domain.Jira;
using System;
using System.Linq.Expressions;

namespace JiraAssistant.Domain.Ui
{
    public class GridColumnInfo
    {
        public string Header { get; set; }
        public string PropertyName { get; set; }
        public bool ApplySecondsToHoursConverter { get; set; }
    }
}
