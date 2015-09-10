using System;

namespace JiraManager.Model
{
   public class JiraIssue
   {
      public string Assignee { get; internal set; }
      public DateTime Created { get; internal set; }
      public string Description { get; internal set; }
      public string Key { get; internal set; }
      public string Priority { get; internal set; }
      public string Project { get; internal set; }
      public string Reporter { get; internal set; }
      public DateTime? Resolved { get; internal set; }
      public string Status { get; internal set; }
      public int StoryPoints { get; internal set; }
      public int Subtasks { get; internal set; }
      public string Summary { get; internal set; }
   }

   public class JiraIssuePrintPreviewModel
   {
      public int Row { get; set; }
      public int Column { get; set; }
      public JiraIssue Issue { get; set; }
   }
}
