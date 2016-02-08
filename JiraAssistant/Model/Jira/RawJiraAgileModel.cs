using System;

namespace JiraAssistant.Model.Jira
{
#pragma warning disable JustCode_CSharp_TypeFileNameMismatch // Types not matching file names
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
      public DateTime? CompleteDate { get; set; }
      public int OriginBoardId { get; set; }
   }

   public class RawAgileEpicsList
   {
      public int MaxResults { get; set; }
      public int StartAt { get; set; }
      public int Total { get; set; }
      public bool IsLast { get; set; }
      public RawAgileEpic[] Values { get; set; }
   }

   public class RawAgileEpic
   {
      public int Id { get; set; }
      public string Key { get; set; }
      public string Self { get; set; }
      public string Name { get; set; }
      public string Summary { get; set; }
      public RawAgileEpicColor Color { get; set; }
      public bool Done { get; set; }
   }

   public class RawAgileEpicColor
   {
      public string Key { get; set; }
   }

   public class RawAgileSprintAssignments
   {
      public string Expand { get; set; }
      public int StartAt { get; set; }
      public int MaxResults { get; set; }
      public int Total { get; set; }
      public RawAgileSprintAssignment[] Issues { get; set; }
   }

   public class RawAgileSprintAssignment
   {
      public string Id { get; set; }
      public string Self { get; set; }
      public string Key { get; set; }
   }

   public class RawAgileBoardFilter
   {
      public int Id { get; set; }
      public string Self { get; set; }
   }

   public class RawAgileIssueStatus
   {
      public string Id { get; set; }
      public string Self { get; set; }
   }

   public class RawAgileBoardColumn
   {
      public string Name { get; set; }
      public RawAgileIssueStatus[] Statuses { get; set; }
      public int? Min { get; set; }
      public int? Max { get; set; }
   }

   public class RawAgileBoardColumnConfig
   {
      public RawAgileBoardColumn[] Columns { get; set; }
      public string ConstraintType { get; set; }
   }

   public class RawAgileIssueField
   {
      public string FieldId { get; set; }
      public string DisplayName { get; set; }
   }

   public class RawAgileBoardEstimation
   {
      public string Type { get; set; }
      public RawAgileIssueField Field { get; set; }
   }

   public class RawAgileBoardRankingField
   {
      public int RankCustomFieldId { get; set; }
   }

   public class RawAgileBoardConfiguration
   {
      public int Id { get; set; }
      public string Name { get; set; }
      public string Self { get; set; }
      public RawAgileBoardFilter Filter { get; set; }
      public RawAgileBoardColumnConfig ColumnConfig { get; set; }
      public RawAgileBoardEstimation Estimation { get; set; }
      public RawAgileBoardRankingField Ranking { get; set; }
   }
#pragma warning restore JustCode_CSharp_TypeFileNameMismatch // Types not matching file names
}
