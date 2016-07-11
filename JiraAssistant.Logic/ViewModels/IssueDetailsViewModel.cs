using JiraAssistant.Domain.Jira;

namespace JiraAssistant.Logic.ViewModels
{
    public class IssueDetailsViewModel
    {
        public IssueDetailsViewModel(JiraIssue issue)
        {
            Issue = issue;
        }

        public JiraIssue Issue { get; private set; }
    }
}
