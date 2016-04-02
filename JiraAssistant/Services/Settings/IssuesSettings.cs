using JiraAssistant.Model.Jira;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;
using Telerik.Windows.Controls;

namespace JiraAssistant.Services.Settings
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

      public ObservableCollection<GridViewDataColumn> SelectedColumns
      {
         get
         {
            if (_selectedColumns == null)
            {
               _selectedColumns = new ObservableCollection<GridViewDataColumn>(SelectedColumnsList.Split(',').Select(i => _allColumns[int.Parse(i)]));
               _selectedColumns.CollectionChanged += (sender, args) =>
               {
                  SelectedColumnsList = string.Join(",", SelectedColumns.Select(c => Array.IndexOf(_allColumns, c)));
               };
            }

            return _selectedColumns;
         }
      }
      public ObservableCollection<GridViewDataColumn> AvailableColumns
      {
         get
         {
            if (_availableColumns == null)
            {
               _availableColumns = new ObservableCollection<GridViewDataColumn>(
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

      private readonly GridViewDataColumn[] _allColumns = {
         /* 00 */new GridViewDataColumn { Header = "Key", DataMemberBinding = new Binding("Key"), IsReadOnly = true },
         /* 01 */new GridViewDataColumn { Header = "Summary", DataMemberBinding = new Binding("Summary"), IsReadOnly = true },
         /* 02 */new GridViewDataColumn { Header = "Reporter", DataMemberBinding = new Binding("Reporter"), IsReadOnly = true },
         /* 03 */new GridViewDataColumn { Header = "Priority", DataMemberBinding = new Binding("Priority"), IsReadOnly = true },
         /* 04 */new GridViewDataColumn { Header = "Status", DataMemberBinding = new Binding("Status"), IsReadOnly = true },
         /* 05 */new GridViewDataColumn { Header = "Created", DataMemberBinding = new Binding("Created"), IsReadOnly = true },
         /* 06 */new GridViewDataColumn { Header = "Story points", DataMemberBinding = new Binding("StoryPoints"), IsReadOnly = true },
         /* 07 */new GridViewDataColumn { Header = "Type", DataMemberBinding = new Binding("BuiltInFields.IssueType.Name"), IsReadOnly = true },
         /* 08 */new GridViewDataColumn { Header = "Resolution", DataMemberBinding = new Binding("BuiltInFields.Resolution.Name"), IsReadOnly = true },
         /* 09 */new GridViewDataColumn { Header = "Assignee", DataMemberBinding = new Binding("Assignee"), IsReadOnly = true },
         /* 10 */new GridViewDataColumn { Header = "Resolved", DataMemberBinding = new Binding("Resolved"), IsReadOnly = true },
         /* 11 */new GridViewDataColumn { Header = "Updated", DataMemberBinding = new Binding("BuiltInFields.Updated"), IsReadOnly = true },
         /* 12 */new GridViewDataColumn { Header = "Epic link", DataMemberBinding = new Binding("EpicLink"), IsReadOnly = true },
         /* 13 */new GridViewDataColumn { Header = "Time spent", DataMemberBinding = new Binding("BuiltInFields.TimeSpent"), IsReadOnly = true },
         /* 14 */new GridViewDataColumn { Header = "Time spent (with subtasks)", DataMemberBinding = new Binding("BuiltInFields.AggregateTimeSpent"), IsReadOnly = true },
         /* 15 */new GridViewDataColumn { Header = "Labels", DataMemberBinding = new Binding("Labels"), IsReadOnly = true },
      };

      private ObservableCollection<GridViewDataColumn> _selectedColumns;
      private ObservableCollection<GridViewDataColumn> _availableColumns;
   }
}
