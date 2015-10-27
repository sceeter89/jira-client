using System;

namespace LightShell.Plugin.Jira.Api.Model
{
   public class RawAgileBoardsList
   {
      public int MaxResults { get; set; }
      public int StartAt { get; set; }
      public bool IsLast { get; set; }
      public RawAgileBoard[] Values { get; set; }
   }

   public class RawAgileBoard
   {
      public int Id { get; set; }
      public string Self { get; set; }
      public string Name { get; set; }
      public string Type { get; set; }
   }


   public class RawAgileSprintsList
   {
      public int MaxResults { get; set; }
      public int StartAt { get; set; }
      public bool IsLast { get; set; }
      public RawAgileSprint[] Values { get; set; }
   }

   public class RawAgileSprint
   {
      public int Id { get; set; }
      public string Self { get; set; }
      public string State { get; set; }
      public string Name { get; set; }
      public DateTime StartDate { get; set; }
      public DateTime EndDate { get; set; }
      public DateTime CompleteDate { get; set; }
      public int OriginBoardId { get; set; }
   }

}
