using JiraAssistant.Domain.Jira;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JiraAssistant.Logic.Dialogs
{
    public partial class LogWorkDialog
    {
        public LogWorkDialog(IEnumerable<JiraIssue> issues)
        {
            InitializeComponent();

            Entries = issues.Select(i => new WorkLogEntry
            {
                Issue = i,
                Hours = 0,
                HoursSpent = TimeSpan.FromSeconds(i.BuiltInFields.TimeSpent ?? 0).TotalHours
            }).ToList();

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
        public double Hours { get; set; }
        public double HoursSpent { get; set; }
        public JiraIssue Issue { get; set; }
    }
}
