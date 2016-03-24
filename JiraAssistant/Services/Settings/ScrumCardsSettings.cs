using JiraAssistant.Model.Jira;
using System;
using System.Windows.Media;

namespace JiraAssistant.Services.Settings
{
   public class ScrumCardsSettings : SettingsBase
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

      public ScrumCardsSettings()
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
                  Labels = new [] { "dev", "prod", "test" },
               }
            }
         };
      }

      public JiraIssuePrintPreviewModel Sample { get; private set; }
   }
}
