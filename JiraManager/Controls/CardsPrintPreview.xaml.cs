using JiraManager.Model;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace JiraManager.Controls
{
   public partial class CardsPrintPreview : UserControl
   {
      private const int Rows = 5;
      private const int Columns = 3;

      private CardsPrintPreview(IEnumerable<JiraIssuePrintPreviewModel> issues)
      {
         InitializeComponent();
         DataContext = issues;
      }

      public static IEnumerable<CardsPrintPreview> GeneratePages(IEnumerable<JiraIssue> issues)
      {
         var issuesLeft = issues;

         while(issuesLeft.Any())
         {
            var issuesForPage = issuesLeft
                                 .Take(Rows * Columns)
                                 .Select((issue, index) => new JiraIssuePrintPreviewModel
                                 {
                                    Issue = issue,
                                    Column = index % Columns,
                                    Row = index / Columns
                                 });
            issuesLeft = issuesLeft.Skip(Rows * Columns);

            yield return new CardsPrintPreview(issuesForPage);
         }
      }
   }
}
