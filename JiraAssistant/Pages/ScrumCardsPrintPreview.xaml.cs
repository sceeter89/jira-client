using GalaSoft.MvvmLight.Command;
using JiraAssistant.Model.Jira;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System;
using JiraAssistant.Dialogs;

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

         StatusBarControl = new ScrumCardsPrintPreviewStatusBar();
         SetCardColorCommand = new RelayCommand<JiraIssuePrintPreviewModel>(SetCardColor);

         DataContext = this;
      }

      private void SetCardColor(JiraIssuePrintPreviewModel printPreview)
      {
         var dialog = new ColorDialog(printPreview.CategoryColor);

         if(dialog.ShowDialog() == false)
            return;

         printPreview.CategoryColor = dialog.EditedColor;
      }

      public RelayCommand<JiraIssuePrintPreviewModel> SetCardColorCommand { get; private set; }

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
