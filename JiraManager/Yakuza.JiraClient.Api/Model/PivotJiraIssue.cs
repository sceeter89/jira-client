using System;

namespace Yakuza.JiraClient.Api.Model
{
   public class PivotJiraIssue
   {
      private readonly JiraIssue _issue;

      public PivotJiraIssue(JiraIssue issue)
      {
         _issue = issue;
      }

      public string Key { get { return _issue.Key; } }
      public string Project { get { return _issue.Project; } }
      public bool IsResolved { get { return _issue.BuiltInFields.Resolution != null; } }
      public DateTime Created { get { return _issue.Created; } }
      public DateTime Resolved { get { return _issue.Resolved ?? DateTime.MinValue; } }
      public string Assignee { get { return _issue.Assignee; } }
      public string Reporter { get { return _issue.Reporter; } }
      public int StoryPoints { get { return _issue.StoryPoints; } }
      public string Priority { get { return _issue.Priority; } }
      public string Type { get { return _issue.BuiltInFields.IssueType.Name; } }
      public string Resolution { get { return (_issue.BuiltInFields.Resolution ?? RawResolution.EmptyResolution).Name; } }
      public int CycleTimeHours
      {
         get
         {
            return IsResolved == false ? 0 :
               (int)(Resolved - Created).TotalHours;
         }
      }
   }
}
