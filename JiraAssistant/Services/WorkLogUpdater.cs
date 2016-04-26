using JiraAssistant.Services.Jira;
using JiraAssistant.Services.Settings;
using System;
using System.Windows.Threading;

namespace JiraAssistant.Services
{
   public class WorkLogUpdater
   {
      private readonly IJiraApi _jiraApi;
      private readonly ReportsSettings _reportsSettings;
      private readonly DispatcherTimer _timer;

      public WorkLogUpdater(ReportsSettings reportsSettings, IJiraApi jiraApi)
      {
         _reportsSettings = reportsSettings;
         _jiraApi = jiraApi;

         _timer = new DispatcherTimer();
         _timer.Interval = TimeSpan.FromSeconds(50);
         _timer.Tick += CheckIfWorkShouldBeLogged;
      }

      private void CheckIfWorkShouldBeLogged(object sender, EventArgs e)
      {
         if (_reportsSettings.RemindAboutWorklog == false)
            return;

         if ((int)DateTime.Now.TimeOfDay.TotalMinutes == (int)_reportsSettings.RemindAt.TimeOfDay.TotalMinutes)
            LogWork();
      }

      private async void LogWork()
      {
         var activeTasks = await _jiraApi.SearchForIssues("Assignee = currentUser() AND Resolution IS EMPTY");

      }
   }
}
