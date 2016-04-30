using JiraAssistant.Model.Jira;
using System.Collections.Generic;
using System.Linq;

namespace JiraAssistant.Dialogs
{
   public partial class LogWorkDialog
   {
      public LogWorkDialog(IEnumerable<JiraIssue> issues)
      {
         InitializeComponent();

         Entries = issues.Select(i => new WorkLogEntry { Issue = i, Hours = 0 }).ToList();

         DataContext = this;
      }

      public IList<WorkLogEntry> Entries { get; private set; }

      private void AcceptClicked(object sender, System.Windows.RoutedEventArgs e)
      {
         DialogResult = true;
      }

      private void CancelClicked(object sender, System.Windows.RoutedEventArgs e)
      {
         DialogResult = false;
      }
   }

   public class WorkLogEntry
   {
      public int Hours { get; set; }
      public JiraIssue Issue { get; set; }
   }
}
