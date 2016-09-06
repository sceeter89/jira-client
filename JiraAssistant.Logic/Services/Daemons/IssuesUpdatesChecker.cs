using GalaSoft.MvvmLight.Command;
using JiraAssistant.Domain.Exceptions;
using JiraAssistant.Logic.Settings;
using JiraAssistant.Logic.Services.Jira;
using NLog;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Telerik.Windows.Controls;
using JiraAssistant.Logic.ContextlessViewModels;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using GalaSoft.MvvmLight.Messaging;
using JiraAssistant.Domain.Messages;
using JiraAssistant.Domain.Jira;

namespace JiraAssistant.Logic.Services.Daemons
{
    public class IssuesUpdatesChecker
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IJiraApi _jiraApi;
        private readonly ReportsSettings _reportsSettings;
        private readonly DispatcherTimer _timer;
        private readonly ImageSourceConverter _imageSourceConverter = new ImageSourceConverter();
        private readonly JiraSessionViewModel _jiraSession;
        private bool _isStationLocked = false;
        private readonly IMessenger _messenger;

        public IssuesUpdatesChecker(ReportsSettings reportsSettings, IJiraApi jiraApi, JiraSessionViewModel jiraSession, IMessenger messenger)
        {
            _reportsSettings = reportsSettings;
            _jiraApi = jiraApi;
            _jiraSession = jiraSession;
            _messenger = messenger;

            SystemEvents.SessionSwitch += (sender, args) =>
            {
                _isStationLocked = args.Reason == SessionSwitchReason.SessionLock;
            };

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(30);
            _timer.Tick += TimerTick;
            _timer.Start();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            if (_jiraSession.IsLoggedIn == false)
                return;

            if (_reportsSettings.MonitorIssuesUpdates == false)
                return;

            if (_isStationLocked)
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
            var projectKeys = _reportsSettings.SelectedProjectsList.Split(',').Select(p => p.Trim());
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

                    if (issue.Created >= changesSince && _reportsSettings.ShowCreatedIssues)
                    {
                        if (_reportsSettings.SkipOwnChanges && issue.BuiltInFields.Reporter.Name == _jiraSession.Profile.Name)
                            continue;

                        alertManager.ShowAlert(new RadDesktopAlert
                        {
                            Header = string.Format("New issue: {0}", issue.Key),
                            Content = issue.Summary,
                            ShowDuration = 5000,
                            Icon = new Image { Source = GetImageFromResources("/Assets/Avatars/PlusSign.png"), Width = 48, Height = 48 },
                            IconColumnWidth = 48,
                            Command = openIssueCommand
                        });
                    }
                    else if (issue.Created < changesSince && _reportsSettings.ShowUpdatedIssues)
                    {
                        var changes = issue.Changelog.Where(ch => ch.Created >= changesSince
                                                                && !(_reportsSettings.SkipOwnChanges
                                                                && ch.Author.Name == _jiraSession.Profile.Name));

                        changes = changes.Concat(issue.BuiltInFields.Comments.Comments.Where(c => c.Updated >= changesSince
                                                                && !(_reportsSettings.SkipOwnChanges
                                                                && c.UpdateAuthor.Name == _jiraSession.Profile.Name))
                                                                .Select(c => new RawChangesHistory
                                                                {
                                                                    Author = c.UpdateAuthor,
                                                                    Created = c.Updated,
                                                                    Items = new[]
                                                                    {
                                                                        new RawChangelogItem
                                                                        {
                                                                            Field = "Comment",
                                                                            Fieldtype = "string",
                                                                            From = "",
                                                                            FromString = "",
                                                                            To = c.Body,
                                                                            toString = c.Body
                                                                        }
                                                                    }
                                                                }));

                        if (changes.Any() == false)
                            continue;

                        _messenger.Send(new IssueUpdatedMessage(issue, changes.SelectMany(c => c.Items), changes.First().Created, changes.First().Author));

                        var rawChangeSummary = new Dictionary<string, string>();

                        foreach (var change in changes)
                        {
                            foreach (var item in change.Items)
                            {
                                rawChangeSummary[item.Field] = item.toString;
                            }
                        }

                        var changeSummaryBuilder = new StringBuilder();
                        foreach (var changeSummary in rawChangeSummary)
                        {
                            var newValue = changeSummary.Value ?? "(None)";
                            changeSummaryBuilder.AppendFormat("{0}: {1}\n", changeSummary.Key, newValue);
                        }

                        alertManager.ShowAlert(new RadDesktopAlert
                        {
                            Header = string.Format("[{0}] {1}", issue.Key, issue.Summary),
                            Content = changeSummaryBuilder.ToString(),
                            ShowDuration = 5000,
                            Icon = new Image { Source = GetImageFromResources("/Assets/Avatars/EditSign.png"), Width = 48, Height = 48 },
                            IconColumnWidth = 48,
                            Command = openIssueCommand
                        });
                    }
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
