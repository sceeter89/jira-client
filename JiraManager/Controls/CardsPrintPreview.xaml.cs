using Yakuza.JiraClient.Model;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;

namespace Yakuza.JiraClient.Controls
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

      private static IEnumerable<CardsPrintPreview> GeneratePages(IEnumerable<JiraIssue> issues)
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

      public static FixedDocument GenerateDocument(IEnumerable<JiraIssue> issues)
      {
         var document = new FixedDocument();
         var pageSize = new Size(8.5 * 96.0, 11.0 * 96.0);

         foreach (var pagePreview in CardsPrintPreview.GeneratePages(issues))
         {
            var pageContent = new PageContent();
            var fixedPage = new FixedPage();
            pagePreview.Height = pageSize.Height - 10;
            pagePreview.Width = pageSize.Width - 10;
            pagePreview.Margin = new Thickness(5);
            pagePreview.UpdateLayout();

            fixedPage.Children.Add(pagePreview);
            ((IAddChild)pageContent).AddChild(fixedPage);
            document.Pages.Add(pageContent);
         }

         return document;
      }
   }
}
