using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace JiraManager.Model
{
   public class RawSearchResults
   {
      public string Expand { get; set; }
      public int StartAt { get; set; }
      public int MaxResults { get; set; }
      public int Total { get; set; }
      public RawIssue[] Issues { get; set; }
   }

   public class RawIssue
   {
      private JToken _fields;

      public string Expand { get; set; }
      public string Id { get; set; }
      public string Self { get; set; }
      public string Key { get; set; }
      [JsonProperty("fields")]
      public JToken RawFields
      {
         get
         {
            return _fields;
         }
         set
         {
            _fields = value;
            BuiltInFields = value.ToObject<RawFields>();
         }
      }

      public RawFields BuiltInFields { get; set; }
   }

   public class RawFields
   {
      public RawIssueType IssueType { get; set; }
      public object TimeSpent { get; set; }
      public RawProjectInfo Project { get; set; }
      public object[] FixVersions { get; set; }
      public object AggregateTimeSpent { get; set; }
      public RawResolution Resolution { get; set; }
      public DateTime? ResolutionDate { get; set; }
      public int Workratio { get; set; }
      public DateTime? LastViewed { get; set; }
      public RawWatches Watches { get; set; }
      public DateTime Created { get; set; }
      public RawPriority Priority { get; set; }
      public object[] Labels { get; set; }
      public object TimeEstimate { get; set; }
      public object AggregateTimeOriginalEstimate { get; set; }
      public object[] Versions { get; set; }
      public RawIssuelink[] IssueLinks { get; set; }
      public RawUserInfo Assignee { get; set; }
      public DateTime Updated { get; set; }
      public RawStatus Status { get; set; }
      public object[] Components { get; set; }
      public object TimeOriginalEstimate { get; set; }
      public string Description { get; set; }
      public object AggregateTimeEstimate { get; set; }
      public string Summary { get; set; }
      public RawUserInfo Creator { get; set; }
      public RawSubtask[] Subtasks { get; set; }
      public RawUserInfo Reporter { get; set; }
      public RawProgressInfo AggregateProgress { get; set; }
      public object Environment { get; set; }
      public object DueDate { get; set; }
      public RawProgressInfo Progress { get; set; }
      public RawVotesInfo Votes { get; set; }
      public RawParent Parent { get; set; }
   }

   public class RawIssueType
   {
      public string Self { get; set; }
      public string Id { get; set; }
      public string Description { get; set; }
      public string IconUrl { get; set; }
      public string Name { get; set; }
      public bool Subtask { get; set; }
   }

   public class RawProjectInfo
   {
      public string Self { get; set; }
      public string Id { get; set; }
      public string Key { get; set; }
      public string Name { get; set; }
      public RawAvatarUrls AvatarUrls { get; set; }
      public RawProjectCategory ProjectCategory { get; set; }
   }

   public class RawAvatarUrls
   {
      public string _48x48 { get; set; }
      public string _24x24 { get; set; }
      public string _16x16 { get; set; }
      public string _32x32 { get; set; }
   }

   public class RawProjectCategory
   {
      public string Self { get; set; }
      public string Id { get; set; }
      public string Description { get; set; }
      public string Name { get; set; }
   }

   public class RawResolution
   {
      public string Self { get; set; }
      public string Id { get; set; }
      public string Description { get; set; }
      public string Name { get; set; }

      public static readonly RawResolution EmptyResolution = new RawResolution
      {
         Self = "N/A",
         Id = "N/A",
         Description = "N /A",
         Name = "N/A"
      };
   }

   public class RawWatches
   {
      public string Self { get; set; }
      public int WatchCount { get; set; }
      public bool IsWatching { get; set; }
   }

   public class RawPriority
   {
      public string Self { get; set; }
      public string IconUrl { get; set; }
      public string Name { get; set; }
      public string Id { get; set; }
   }

   public class RawStatus
   {
      public string Self { get; set; }
      public string Description { get; set; }
      public string IconUrl { get; set; }
      public string Name { get; set; }
      public string Id { get; set; }
      public RawStatusCategory StatusCategory { get; set; }
   }

   public class RawStatusCategory
   {
      public string Self { get; set; }
      public int Id { get; set; }
      public string Key { get; set; }
      public string ColorName { get; set; }
      public string Name { get; set; }
   }

   public class RawUserInfo
   {
      public string Self { get; set; }
      public string Name { get; set; }
      public string Key { get; set; }
      public string EmailAddress { get; set; }
      public RawAvatarUrls AvatarUrls { get; set; }
      public string DisplayName { get; set; }
      public bool Active { get; set; }
      public string TimeZone { get; set; }

      public static readonly RawUserInfo EmptyInfo = new RawUserInfo
      {
         Self = "N/A",
         Key = "N/A",
         EmailAddress = "N/A",
         Active = false,
         TimeZone = "N/A",
         DisplayName = "N/A"
      };
   }

   public class RawProgressInfo
   {
      public int Progress { get; set; }
      public int Total { get; set; }
   }

   public class RawVotesInfo
   {
      public string Self { get; set; }
      public int Votes { get; set; }
      public bool HasVoted { get; set; }
   }

   public class RawParent
   {
      public string Id { get; set; }
      public string Key { get; set; }
      public string Self { get; set; }
      public RawRelatedIssueFields Fields { get; set; }
   }

   public class RawRelatedIssueFields
   {
      public string Summary { get; set; }
      public RawStatus Status { get; set; }
      public RawPriority Priority { get; set; }
      public RawIssueType Issuetype { get; set; }
   }

   public class RawIssuelink
   {
      public string Id { get; set; }
      public string Self { get; set; }
      public RawLinkType Type { get; set; }
      public RawInwardIssue InwardIssue { get; set; }
      public RawOutwardIssue OutwardIssue { get; set; }
   }

   public class RawLinkType
   {
      public string Id { get; set; }
      public string Name { get; set; }
      public string Inward { get; set; }
      public string Outward { get; set; }
      public string Self { get; set; }
   }

   public class RawInwardIssue
   {
      public string Id { get; set; }
      public string Key { get; set; }
      public string Self { get; set; }
      public RawRelatedIssueFields Fields { get; set; }
   }

   public class RawOutwardIssue
   {
      public string Id { get; set; }
      public string Key { get; set; }
      public string Self { get; set; }
      public RawRelatedIssueFields Fields { get; set; }
   }

   public class RawSubtask
   {
      public string Id { get; set; }
      public string Key { get; set; }
      public string Self { get; set; }
      public RawRelatedIssueFields Fields { get; set; }
   }

   public class RawSessionInfo
   {
      public string Self { get; set; }
      public string Name { get; set; }
      public RawLoginInfo LoginInfo { get; set; }
   }

   public class RawLoginInfo
   {
      public int FailedLoginCount { get; set; }
      public int LoginCount { get; set; }
      public DateTime LastFailedLoginTime { get; set; }
      public DateTime PreviousLoginTime { get; set; }
   }

   public class RawSuccessfulLoginParameters
   {
      public RawSession Session { get; set; }
      public RawLogininfo LoginInfo { get; set; }
   }

   public class RawSession
   {
      public string Name { get; set; }
      public string Value { get; set; }
   }

   public class RawLogininfo
   {
      public int FailedLoginCount { get; set; }
      public int LoginCount { get; set; }
      public DateTime LastFailedLoginTime { get; set; }
      public DateTime PreviousLoginTime { get; set; }
   }

   public class RawFieldDefinition
   {
      public string Id { get; set; }
      public string Name { get; set; }
      public bool Custom { get; set; }
      public bool Orderable { get; set; }
      public bool Navigable { get; set; }
      public bool Searchable { get; set; }
      public string[] ClauseNames { get; set; }
      public RawFieldSchema Schema { get; set; }
   }

   public class RawFieldSchema
   {
      public string Type { get; set; }
      public string System { get; set; }
      public string Items { get; set; }
      public string Custom { get; set; }
      public int CustomId { get; set; }
   }
}
