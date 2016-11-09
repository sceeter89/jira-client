using JiraAssistant.Domain.Jira;
using JiraAssistant.Logic.Services.Jira;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JiraAssistant.Controls.Dialogs
{
    public partial class LogWorkDialog
    {
        private readonly IJiraApi _jiraApi;

        public LogWorkDialog(IEnumerable<JiraIssue> issues, IJiraApi jiraApi)
        {
            InitializeComponent();
            _jiraApi = jiraApi;

            Entries = issues.Select(i => new WorkLogEntry
            {
                Issue = i,
                Hours = 0,
                HoursSpent = TimeSpan.FromSeconds(i.BuiltInFields.TimeSpent ?? 0).TotalHours
            }).ToList();

            DataContext = this;
        }

        public IList<WorkLogEntry> Entries { get; private set; }

        private async void AcceptClicked(object sender, System.Windows.RoutedEventArgs args)
        {
            foreach (var entry in Entries.Where(e => e.Hours > 0))
            {
                await _jiraApi.Worklog.Log(entry.Issue, entry.Hours);
            }
            DialogResult = true;
        }

        private void CancelClicked(object sender, System.Windows.RoutedEventArgs args)
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
