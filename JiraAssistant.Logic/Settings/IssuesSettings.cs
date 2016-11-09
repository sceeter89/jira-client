using JiraAssistant.Domain.Jira;
using JiraAssistant.Domain.Ui;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;

namespace JiraAssistant.Logic.Settings
{
    public class IssuesSettings : SettingsBase
    {
        public bool ShowStoryPoints
        {
            get { return GetValue(true); }
            set { SetValue(value, true); }
        }

        public bool ShowIssueType
        {
            get { return GetValue(false); }
            set { SetValue(value, false); }
        }

        public string SelectedColumnsList
        {
            get { return GetValue("0,1,2,3,4,6,7,8,10,12,13"); }
            set { SetValue(value, "0,1,2,3,4,6,7,8,10,12,13"); }
        }

        public ObservableCollection<GridColumnInfo> SelectedColumns
        {
            get
            {
                if (_selectedColumns == null)
                {
                    _selectedColumns = new ObservableCollection<GridColumnInfo>(SelectedColumnsList.Split(',').Select(i => _allColumns[int.Parse(i)]));
                    _selectedColumns.CollectionChanged += (sender, args) =>
                    {
                        SelectedColumnsList = string.Join(",", SelectedColumns.Select(c => Array.IndexOf(_allColumns, c)));
                    };
                }

                return _selectedColumns;
            }
        }

        public ObservableCollection<GridColumnInfo> AvailableColumns
        {
            get
            {
                if (_availableColumns == null)
                {
                    _availableColumns = new ObservableCollection<GridColumnInfo>(
                       Enumerable.Range(0, _allColumns.Length)
                          .Except(SelectedColumnsList.Split(',').Select(i => int.Parse(i)))
                          .Select(i => _allColumns[i])
                       );
                }
                return _availableColumns;
            }
        }

        public IssuesSettings()
        {
            Sample = new JiraIssuePrintPreviewModel
            {
                CategoryColor = Colors.LightCoral,
                Issue = new JiraIssue
                {
                    Key = "PRJ-1234",
                    EpicLink = "PRJ-4321",
                    Summary = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.",
                    Project = "ProjecT",
                    Assignee = "John Smith",
                    Created = new DateTime(2016, 2, 1),
                    Priority = "Major",
                    Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.",
                    Status = "Open",
                    Reporter = "Louis Kovalsky",
                    Resolved = new DateTime(2016, 3, 1),
                    Subtasks = 3,
                    SprintIds = new[] { 1, 2, 3 },
                    StoryPoints = 5,
                    BuiltInFields = new RawFields
                    {
                        IssueType = new RawIssueType { Name = "User Story" },
                        Resolution = new RawResolution { Name = "Fixed" },
                        DueDate = new DateTime(2016, 2, 20),
                        Labels = new[] { "dev", "prod", "test" },
                    }
                }
            };
        }

        public JiraIssuePrintPreviewModel Sample { get; private set; }

        private readonly GridColumnInfo[] _allColumns = {
         /* 00 */new GridColumnInfo { Header = "Key", PropertyName ="Key" },
         /* 01 */new GridColumnInfo { Header = "Summary", PropertyName ="Summary" },
         /* 02 */new GridColumnInfo { Header = "Reporter", PropertyName = "Reporter" },
         /* 03 */new GridColumnInfo { Header = "Priority", PropertyName = "Priority" },
         /* 04 */new GridColumnInfo { Header = "Status", PropertyName = "Status" },
         /* 05 */new GridColumnInfo { Header = "Created", PropertyName = "Created" },
         /* 06 */new GridColumnInfo { Header = "Story points", PropertyName = "StoryPoints" },
         /* 07 */new GridColumnInfo { Header = "Type", PropertyName = "BuiltInFields.IssueType.Name" },
         /* 08 */new GridColumnInfo { Header = "Resolution", PropertyName = "BuiltInFields.Resolution.Name" },
         /* 09 */new GridColumnInfo { Header = "Assignee", PropertyName = "Assignee" },
         /* 10 */new GridColumnInfo { Header = "Resolved", PropertyName = "Resolved" },
         /* 11 */new GridColumnInfo { Header = "Updated", PropertyName = "BuiltInFields.Updated" },
         /* 12 */new GridColumnInfo { Header = "Epic link", PropertyName = "EpicLink" },
         /* 13 */new GridColumnInfo { Header = "Time spent [h]", PropertyName = "BuiltInFields.TimeSpent", ApplySecondsToHoursConverter = true },
         /* 14 */new GridColumnInfo { Header = "Time spent (with subtasks) [h]", PropertyName = "BuiltInFields.AggregateTimeSpent", ApplySecondsToHoursConverter = true },
         /* 15 */new GridColumnInfo { Header = "Labels", PropertyName = "Labels" },
         /* 16 */new GridColumnInfo { Header = "Epic name", PropertyName ="EpicName" },
      };

        private ObservableCollection<GridColumnInfo> _selectedColumns;
        private ObservableCollection<GridColumnInfo> _availableColumns;
    }
}
