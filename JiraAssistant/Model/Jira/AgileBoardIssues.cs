using System.Collections.Generic;
using System.Linq;

namespace JiraAssistant.Model.Jira
{
   public class AgileBoardIssues
   {
      private readonly IDictionary<string, JiraIssue> _issuesByKey;
      private readonly IDictionary<int, IList<JiraIssue>> _issuesBySprint = new Dictionary<int, IList<JiraIssue>>();

      public AgileBoardIssues(IList<JiraIssue> issues, IList<RawAgileEpic> epics, IList<RawAgileSprint> sprints)
      {
         Issues = issues;
         Epics = epics;
         Sprints = sprints;

         _issuesByKey = issues.ToDictionary(i => i.Key, i => i);

         foreach (var issue in issues)
         {
            foreach (var sprintId in issue.SprintIds)
            {
               if (_issuesBySprint.ContainsKey(sprintId) == false)
                  _issuesBySprint[sprintId] = new List<JiraIssue>();

               _issuesBySprint[sprintId].Add(issue);
            }
         }
      }

      public IList<JiraIssue> Issues { get; private set; }
      public IList<RawAgileEpic> Epics { get; private set; }
      public IList<RawAgileSprint> Sprints { get; private set; }

      public JiraIssue IssueByKey(string issueKey)
      {
         return _issuesByKey[issueKey];
      }

      public IList<JiraIssue> IssuesInSprint(int sprintId)
      {
         return _issuesBySprint[sprintId];
      }
   }
}
