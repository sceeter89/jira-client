using JiraManager.Model;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace JiraManager.Helpers
{
   public class RawIssueToJiraIssue
   {
      private readonly IDictionary<string, RawFieldDefinition> _fields;

      public RawIssueToJiraIssue(IEnumerable<RawFieldDefinition> definitions)
      {
         _fields = definitions.ToDictionary(d => d.Name, d => d);
      }

      public JiraIssue Convert(RawIssue issue)
      {
         return new JiraIssue
         {
            Key = issue.Key,
            Project = GetFieldByName<string>(issue, "Project", "name"),
            Summary = GetFieldByName<string>(issue, "Summary"),
            Priority = GetFieldByName<string>(issue, "Priority", "name"),
            StoryPoints = (int)(GetFieldByName<float?>(issue, "Story Points") ?? 0),
            Subtasks = issue.Fields["subtasks"].Count()
         };
      }

      private T GetFieldByName<T>(RawIssue issue, string fieldName, string path = null)
      {
         if (_fields.ContainsKey(fieldName) == false)
            return default(T);

         var fieldId = _fields[fieldName].Id;
         JToken token;
         if (path == null)
            return issue.Fields.Value<T>(fieldId);
         else
         {
            token = issue.Fields.SelectToken(fieldId);
            foreach (var part in path.Split('/'))
            {
               token = token.SelectToken(part);
            }
         }

         if (token == null)
            return default(T);

         return token.ToObject<T>();
      }
   }
}
