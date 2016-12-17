using JiraAssistant.Domain.Jira;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace JiraAssistant.Logic.Settings
{
    public class ReportsSettings : SettingsBase
    {
        private readonly IJiraApi _jiraApi;
        private ObservableCollection<RawProjectInfo> _selectedProjects;
        private RawProjectInfo[] _allProjects;
        private ObservableCollection<RawProjectInfo> _availableProjects;

        public ReportsSettings(IJiraApi jiraApi)
        {
            _jiraApi = jiraApi;
            GetProjects();
        }

        public bool RemindAboutWorklog
        {
            get { return GetValue(defaultValue: false); }
            set { SetValue(value, defaultValue: false); }
        }

        public DateTime RemindAt
        {
            get { return GetValue(defaultValue: new DateTime(1, 1, 1, 16, 0, 0)); }
            set { SetValue(value, defaultValue: new DateTime(1, 1, 1, 16, 0, 0)); }
        }

        public DateTime LastLogWorkDisplayed
        {
            get { return GetValue(defaultValue: new DateTime(1900, 1, 1)); }
            set { SetValue(value, defaultValue: new DateTime(1900, 1, 1)); }
        }

        public bool MonitorIssuesUpdates
        {
            get { return GetValue(defaultValue: false); }
            set { SetValue(value, defaultValue: false); }
        }

        public bool ShowCreatedIssues
        {
            get { return GetValue(defaultValue: true); }
            set { SetValue(value, defaultValue: true); }
        }

        public bool ShowUpdatedIssues
        {
            get { return GetValue(defaultValue: true); }
            set { SetValue(value, defaultValue: true); }
        }

        public TimeSpan ScanForUpdatesInterval
        {
            get { return GetValue(defaultValue: TimeSpan.FromMinutes(5)); }
            set { SetValue(value, defaultValue: TimeSpan.FromMinutes(5)); }
        }

        public DateTime LastUpdatesScan
        {
            get { return GetValue(defaultValue: new DateTime(1900, 1, 1)); }
            set { SetValue(value, defaultValue: new DateTime(1900, 1, 1)); }
        }

        public string SelectedProjectsList
        {
            get { return GetValue(defaultValue: ""); }
            set { SetValue(value, defaultValue: ""); }
        }

        public ObservableCollection<RawProjectInfo> SelectedProjects
        {
            get
            {
                if (_allProjects == null)
                    return null;

                if (_selectedProjects == null)
                {
                    var selectedProjectsKeys = new HashSet<string>(SelectedProjectsList.Split(','));
                    _selectedProjects = new ObservableCollection<RawProjectInfo>(_allProjects.Where(p => selectedProjectsKeys.Contains(p.Key)));
                    _selectedProjects.CollectionChanged += (sender, args) =>
                    {
                        SelectedProjectsList = string.Join(",", SelectedProjects.Select(p => p.Key));
                    };
                }

                return _selectedProjects;
            }
        }

        private async void GetProjects()
        {
            try
            {
                var result = await _jiraApi.Server.GetProjects();
                if (result == null)
                    return;
                _allProjects = result.ToArray();
                RaisePropertyChanged("SelectedProjects");
                RaisePropertyChanged("AvailableProjects");
            }
            catch { }
        }

        public ObservableCollection<RawProjectInfo> AvailableProjects
        {
            get
            {
                if (_allProjects == null)
                    return null;

                if (_availableProjects == null)
                {
                    var selectedProjectsKeys = new HashSet<string>(SelectedProjectsList.Split(','));
                    _availableProjects = new ObservableCollection<RawProjectInfo>(_allProjects.Where(p => selectedProjectsKeys.Contains(p.Key) == false));
                }

                return _availableProjects;
            }
        }

        public bool SkipOwnChanges
        {
            get { return GetValue(defaultValue: true); }
            set { SetValue(value, defaultValue: true); }
        }
    }
}
