using JiraAssistant.Model.Jira;

namespace JiraAssistant.Pages
{
   public partial class IssueDetailsPage
   {
      public IssueDetailsPage(JiraIssue issue)
      {
         InitializeComponent();

         Issue = issue;

         DataContext = this;
      }

      public JiraIssue Issue { get; private set; }
   }
}
