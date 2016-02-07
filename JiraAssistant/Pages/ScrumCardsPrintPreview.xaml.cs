using JiraAssistant.Model.Jira;
using System.Collections.Generic;
using System.Linq;

namespace JiraAssistant.Pages
{
   public partial class ScrumCardsPrintPreview : BaseNavigationPage
   {
      private const int Rows = 5;
      private const int Columns = 3;

      public ScrumCardsPrintPreview(IEnumerable<JiraIssue> issues)
      {
         InitializeComponent();

         Pages = new List<PrintPage>();
         var issuesLeft = issues;

         while (issuesLeft.Any())
         {
            var issuesForPage = 
               issuesLeft.Take(Rows * Columns)
                         .Select((issue, index) => new JiraIssuePrintPreviewModel
                         {
                            Issue = issue,
                            Column = index % Columns,
                            Row = index / Columns
                         });
            issuesLeft = issuesLeft.Skip(Rows * Columns);

            Pages.Add(new PrintPage(issuesForPage));
         }

         DataContext = this;

      }

      public IList<PrintPage> Pages { get; private set; }
   }

   public class PrintPage
   {
      public PrintPage(IEnumerable<JiraIssuePrintPreviewModel> issuesForPage)
      {
         Issues = issuesForPage;
      }

      public IEnumerable<JiraIssuePrintPreviewModel> Issues { get; private set; }
   }
}
