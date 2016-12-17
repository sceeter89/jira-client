using JiraAssistant.Domain.Exceptions;
using JiraAssistant.Logic.Settings;
using JiraAssistant.Domain.Jira;
using NLog;
using System;
using System.Diagnostics;
using System.Linq;
using JiraAssistant.Logic.ContextlessViewModels;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using GalaSoft.MvvmLight.Messaging;
using JiraAssistant.Domain.Messages;
using System.Timers;

namespace JiraAssistant.Logic.Services.Daemons
{
    public class IssuesUpdatesChecker
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IJiraApi _jiraApi;
        private readonly ReportsSettings _reportsSettings;
        private readonly Timer _timer;
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

            _timer = new Timer();
            _timer.Interval = TimeSpan.FromSeconds(30).TotalMilliseconds;
            _timer.Elapsed += TimerTick;
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
                    if (issue.Created >= changesSince && _reportsSettings.ShowCreatedIssues)
                    {
                        if (_reportsSettings.SkipOwnChanges && issue.BuiltInFields.Reporter.Name == _jiraSession.Profile.Name)
                            continue;

                        _messenger.Send(new ShowDesktopNotificationMessage(
                            title: string.Format("New issue: {0}", issue.Key),
                            description: issue.Summary,
                            clickCallback: () =>
                            {
                                var i = issue;
                                Process.Start(string.Format("{0}/browse/{1}", _jiraApi.Server.ServerUri, i.Key));
                            },
                            iconResource: "/Assets/Avatars/PlusSign.png"));
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
                        
                        _messenger.Send(new ShowDesktopNotificationMessage(
                            title: string.Format("[{0}] {1}", issue.Key, issue.Summary),
                            description: changeSummaryBuilder.ToString(),
                            clickCallback: () =>
                            {
                                var i = issue;
                                Process.Start(string.Format("{0}/browse/{1}", _jiraApi.Server.ServerUri, i.Key));
                            },
                            iconResource: "/Assets/Avatars/EditSign.png"));
                    }
                }
                _reportsSettings.LastUpdatesScan = DateTime.Now;
            }
            catch (SearchFailedException e)
            {
                _logger.Log(LogLevel.Warn, "Failed to retrieve updated issues.", e);
                _messenger.Send(new ShowDesktopNotificationMessage(
                            title: "Invalid configuration",
                            description: "Posible causes: incorrect list of project keys, credentials expired, connectivity issues.",
                            clickCallback: null,
                            iconResource: null));
            }
        }
    }
}
