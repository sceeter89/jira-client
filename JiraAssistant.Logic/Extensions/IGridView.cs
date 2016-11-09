using JiraAssistant.Domain.Jira;
using System.Collections.Generic;

namespace JiraAssistant.Logic.Extensions
{
    public interface IGridView
    {
        void SaveGridStateTo();
        void LoadGridStateFrom();
        IEnumerable<JiraIssue> GetFilteredIssues();
    }
}
