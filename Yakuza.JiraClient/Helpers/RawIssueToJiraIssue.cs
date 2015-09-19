using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Yakuza.JiraClient.Api.Model;

namespace Yakuza.JiraClient.Helpers
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
            Project = issue.BuiltInFields.Project.Name,
            Summary = issue.BuiltInFields.Summary,
            Priority = issue.BuiltInFields.Priority.Name,
            StoryPoints = (int)(GetFieldByName<float?>(issue, "Story Points") ?? 0),
            Subtasks = issue.BuiltInFields.Subtasks.Count(),
            Created = issue.BuiltInFields.Created,
            Resolved = issue.BuiltInFields.ResolutionDate ?? DateTime.MinValue,
            Status = issue.BuiltInFields.Status.Name,
            Description = issue.BuiltInFields.Description,
            Assignee = (issue.BuiltInFields.Assignee ?? RawUserInfo.EmptyInfo).DisplayName,
            Reporter = (issue.BuiltInFields.Reporter ?? RawUserInfo.EmptyInfo).DisplayName,
            BuiltInFields = issue.BuiltInFields
         };
      }

      private T GetFieldByName<T>(RawIssue issue, string fieldName, string path = null)
      {
         if (_fields.ContainsKey(fieldName) == false)
            return default(T);

         var fieldId = _fields[fieldName].Id;
         JToken token;
         if (path == null)
            return issue.RawFields.Value<T>(fieldId);
         else
         {
            token = issue.RawFields.SelectToken(fieldId);
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
