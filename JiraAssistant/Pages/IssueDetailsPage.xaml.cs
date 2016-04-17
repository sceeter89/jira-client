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
      public override string Title { get { return string.Format("Issue: {0}", Issue.Key); } }
   }
}
