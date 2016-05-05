using GalaSoft.MvvmLight.Command;
using JiraAssistant.Model.Exceptions;
using JiraAssistant.Services.Jira;
using JiraAssistant.Services.Settings;
using JiraAssistant.ViewModel;
using NLog;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Telerik.Windows.Controls;

namespace JiraAssistant.Services
{
   public class IssuesUpdatesChecker
   {
      private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
      private readonly IJiraApi _jiraApi;
      private readonly ReportsSettings _reportsSettings;
      private readonly DispatcherTimer _timer;
      private readonly ImageSourceConverter _imageSourceConverter = new ImageSourceConverter();
      private readonly JiraSessionViewModel _jiraSession;

      public IssuesUpdatesChecker(ReportsSettings reportsSettings, IJiraApi jiraApi, JiraSessionViewModel jiraSession)
      {
         _reportsSettings = reportsSettings;
         _jiraApi = jiraApi;
         _jiraSession = jiraSession;

         _timer = new DispatcherTimer();
         _timer.Interval = TimeSpan.FromSeconds(30);
         _timer.Tick += TimerTick;
         _timer.Start();
      }

      private void TimerTick(object sender, EventArgs e)
      {
         if (_jiraSession.IsLoggedIn == false)
            return;

         if (_reportsSettings.ShowUpdatedIssues == false)
            return;

         if (DateTime.Now - _reportsSettings.LastUpdatesScan > _reportsSettings.ScanForUpdatesInterval)
            ScanForUpdates();
      }

      private DateTime GreatestDateTime(params DateTime[] dateTimes)
      {
         return dateTimes.Max();
      }

      private async void ScanForUpdates()
      {
         var alertManager = new RadDesktopAlertManager();

         var changesSince = GreatestDateTime((DateTime.Now - TimeSpan.FromHours(24)), _reportsSettings.LastUpdatesScan);
         var projectKeys = _reportsSettings.ProjectsList.Split(',').Select(p => p.Trim());
         try
         {
            var query = string.Format("updated >= '{0}'", changesSince.ToString("yyyy-MM-dd HH:mm"));
            if (projectKeys.Any())
               query += string.Format(" AND project IN ({0})", string.Join(",", projectKeys));

            var updatedIssues = await _jiraApi.SearchForIssues(query);
            foreach (var issue in updatedIssues.Where(i => i.BuiltInFields.Updated >= _reportsSettings.LastUpdatesScan))
            {
               var openIssueCommand = new RelayCommand(() =>
               {
                  var i = issue;
                  Process.Start(string.Format("{0}/browse/{1}", _jiraApi.Server.ServerUri, i.Key));
               });

               if (issue.Created >= changesSince)
                  alertManager.ShowAlert(new RadDesktopAlert
                  {
                     Header = "New issue: " + issue.Key,
                     Content = issue.Summary,
                     ShowDuration = 5000,
                     Icon = new Image { Source = GetImageFromResources("/Assets/Avatars/PlusSign.png"), Width = 48, Height = 48 },
                     IconColumnWidth = 48,
                     Command = openIssueCommand
                  });
               else
                  alertManager.ShowAlert(new RadDesktopAlert
                  {
                     Header = "Updated: " + issue.Key,
                     Content = issue.Summary,
                     ShowDuration = 5000,
                     Icon = new Image { Source = GetImageFromResources("/Assets/Avatars/EditSign.png"), Width = 48, Height = 48 },
                     IconColumnWidth = 48,
                     Command = openIssueCommand
                  });
            }
            _reportsSettings.LastUpdatesScan = DateTime.Now;
         }
         catch (SearchFailedException e)
         {
            _logger.Log(LogLevel.Warn, "Failed to retrieve updated issues.", e);
            alertManager.ShowAlert(new RadDesktopAlert
            {
               Header = "Invalid configuration",
               Content = "Please verify that you provided correct list of project keys",
               ShowDuration = 2000
            });
         }
      }

      protected ImageSource GetImageFromResources(string path)
      {
         var uri = string.Format("pack://application:,,,/JiraAssistant;component/{0}", path.TrimStart('/'));
         return _imageSourceConverter.ConvertFromString(uri) as ImageSource;
      }
   }
}
