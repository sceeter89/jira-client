using GalaSoft.MvvmLight.Command;
using JiraAssistant.Logic.Settings;
using JiraAssistant.Logic.Services.Jira;
using System;
using System.Linq;
using System.Windows.Threading;
using JiraAssistant.Domain.Exceptions;
using GalaSoft.MvvmLight.Messaging;
using JiraAssistant.Domain.Messages.Dialogs;

namespace JiraAssistant.Logic.Services.Daemons
{
    public class WorkLogUpdater
    {
        private readonly IJiraApi _jiraApi;
        private readonly ReportsSettings _reportsSettings;
        private readonly DispatcherTimer _timer;
        private bool _popupOpened;
        private readonly IMessenger _messenger;

        public WorkLogUpdater(ReportsSettings reportsSettings, IJiraApi jiraApi, IMessenger messenger)
        {
            _reportsSettings = reportsSettings;
            _jiraApi = jiraApi;
            _messenger = messenger;

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

            if (DateTime.Now >= todayDisplayTime && _reportsSettings.LastLogWorkDisplayed < todayDisplayTime)
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
                if (activeTasks.Any() == false)
                    return;

                _messenger.Send(new OpenWorklogMessage(activeTasks));
            }
            catch (IncompleteJiraConfiguration) { }
            finally
            {
                _popupOpened = false;
            }
        }
    }
}
