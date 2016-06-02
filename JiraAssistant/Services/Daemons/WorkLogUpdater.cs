using GalaSoft.MvvmLight.Command;
using JiraAssistant.Dialogs;
using JiraAssistant.Services.Jira;
using JiraAssistant.Settings;
using System;
using System.Linq;
using System.Windows.Threading;

namespace JiraAssistant.Services.Daemons
{
    public class WorkLogUpdater
    {
        private readonly IJiraApi _jiraApi;
        private readonly ReportsSettings _reportsSettings;
        private readonly DispatcherTimer _timer;
        private bool _popupOpened;

        public WorkLogUpdater(ReportsSettings reportsSettings, IJiraApi jiraApi)
        {
            _reportsSettings = reportsSettings;
            _jiraApi = jiraApi;

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(30);
            _timer.Tick += CheckIfWorkShouldBeLogged;
            _timer.Start();

            LogWorkCommand = new RelayCommand(LogWork);
        }

        public RelayCommand LogWorkCommand { get; private set; }

        private void CheckIfWorkShouldBeLogged(object sender, EventArgs e)
        {
            if (_reportsSettings.RemindAboutWorklog == false)
                return;

            var todayDisplayTime = DateTime.Today.Add(_reportsSettings.RemindAt.TimeOfDay);

            if (_reportsSettings.LastLogWorkDisplayed <= todayDisplayTime)
            {
                _reportsSettings.LastLogWorkDisplayed = DateTime.Now;
                LogWork();
            }
        }

        private async void LogWork()
        {
            if (_popupOpened)
                return;

            try
            {
                _popupOpened = true;
                var activeTasks = await _jiraApi.SearchForIssues("Assignee = currentUser() AND (Resolution IS EMPTY OR (resolved >= \"-24h\" AND resolved < endOfDay()))");
                var dialog = new LogWorkDialog(activeTasks);
                if (dialog.ShowDialog() == false)
                    return;

                foreach (var entry in dialog.Entries.Where(e => e.Hours > 0))
                {
                    await _jiraApi.Worklog.Log(entry.Issue, entry.Hours);
                }
            }
            finally
            {
                _popupOpened = false;
            }
        }
    }
}
