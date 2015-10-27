using System;

namespace LightShell.Plugin.Jira.Api.Model
{
   public class JiraIssue
   {
      public string Assignee { get; set; }
      public DateTime Created { get; set; }
      public string Description { get; set; }
      public string Key { get; set; }
      public string Priority { get; set; }
      public string Project { get; set; }
      public string Reporter { get; set; }
      public DateTime? Resolved { get; set; }
      public string Status { get; set; }
      public int StoryPoints { get; set; }
      public int Subtasks { get; set; }
      public string Summary { get; set; }
      public RawFields BuiltInFields { get; set; }
   }

   public class JiraIssuePrintPreviewModel
   {
      public int Row { get; set; }
      public int Column { get; set; }
      public JiraIssue Issue { get; set; }
   }
}
