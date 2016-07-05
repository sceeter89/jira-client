using System;

namespace JiraAssistant.Domain.Jira
{
    public class PivotJiraIssue
    {
        public PivotJiraIssue(JiraIssue issue)
        {
            Key = issue.Key;
            Project = issue.Project;
            IsResolved = issue.BuiltInFields.Resolution != null;
            Created = issue.Created;
            Resolved = issue.Resolved ?? DateTime.MinValue;
            Assignee = issue.Assignee;
            Reporter = issue.Reporter;
            StoryPoints = issue.StoryPoints;
            Priority = issue.Priority;
            Type = issue.BuiltInFields.IssueType.Name;
            Resolution = (issue.BuiltInFields.Resolution ?? RawResolution.EmptyResolution).Name;
            EpicName = issue.EpicName;
        }

        public string Key { get; private set; }
        public string Project { get; private set; }
        public bool IsResolved { get; private set; }
        public DateTime Created { get; private set; }
        public DateTime Resolved { get; private set; }
        public string Assignee { get; private set; }
        public string Reporter { get; private set; }
        public float StoryPoints { get; private set; }
        public string Priority { get; private set; }
        public string Type { get; private set; }
        public string Resolution { get; private set; }
        public int CycleTimeHours
        {
            get
            {
                return IsResolved == false ? 0 :
                   (int)(Resolved - Created).TotalHours;
            }
        }
        public string EpicName { get; set; }
    }
}
