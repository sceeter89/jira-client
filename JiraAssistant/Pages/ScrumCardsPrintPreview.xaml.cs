using JiraAssistant.Model.Jira;
using System.Collections.Generic;

namespace JiraAssistant.Pages
{
   public partial class ScrumCardsPrintPreview : BaseNavigationPage
   {
      public ScrumCardsPrintPreview(IEnumerable<JiraIssue> issues)
      {
         InitializeComponent();

         Issues = issues;

         DataContext = this;
      }

      public IEnumerable<JiraIssue> Issues { get; private set; }
   }
}
