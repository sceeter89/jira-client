using System;

namespace JiraManager.Model
{
   public class SearchResults
   {
      public string Expand { get; set; }
      public int StartAt { get; set; }
      public int MaxResults { get; set; }
      public int Total { get; set; }
      public Issue[] Issues { get; set; }
   }

   public class Issue
   {
      public string Expand { get; set; }
      public string Id { get; set; }
      public string Self { get; set; }
      public string Key { get; set; }
      public Fields Fields { get; set; }
   }

   public class Fields
   {
      public IssueType IssueType { get; set; }
      public object TimeSpent { get; set; }
      public ProjectInfo Project { get; set; }
      public string Customfield_11000 { get; set; }
      public object[] FixVersions { get; set; }
      public object Customfield_11200 { get; set; }
      public object AggregateTimeSpent { get; set; }
      public Resolution Resolution { get; set; }
      public object[] Customfield_11201 { get; set; }
      public object Customfield_11004 { get; set; }
      public object Customfield_11202 { get; set; }
      public string Customfield_10500 { get; set; }
      public object Customfield_10700 { get; set; }
      public object Customfield_10701 { get; set; }
      public object Customfield_10702 { get; set; }
      public object Customfield_10703 { get; set; }
      public Customfield_10901 Customfield10901 { get; set; }
      public DateTime? ResolutionDate { get; set; }
      public int Workratio { get; set; }
      public DateTime? LastViewed { get; set; }
      public Watches Watches { get; set; }
      public DateTime Created { get; set; }
      public Priority Priority { get; set; }
      public object Customfield_10100 { get; set; }
      public object Customfield_10101 { get; set; }
      public object Customfield_10102 { get; set; }
      public object[] Labels { get; set; }
      public object Customfield_10103 { get; set; }
      public object TimeEstimate { get; set; }
      public object AggregateTimeOriginalEstimate { get; set; }
      public object[] Versions { get; set; }
      public Issuelink[] IssueLinks { get; set; }
      public UserInfo Assignee { get; set; }
      public DateTime Updated { get; set; }
      public Status Status { get; set; }
      public object[] Components { get; set; }
      public object TimeOriginalEstimate { get; set; }
      public string Description { get; set; }
      public string Customfield_11100 { get; set; }
      public object Customfield_10203 { get; set; }
      public object Customfield_10204 { get; set; }
      public string[] Customfield_10600 { get; set; }
      public object Customfield_10205 { get; set; }
      public object Customfield_10800 { get; set; }
      public object AggregateTimeEstimate { get; set; }
      public string Summary { get; set; }
      public UserInfo Creator { get; set; }
      public Subtask[] Subtasks { get; set; }
      public UserInfo Reporter { get; set; }
      public ProgressInfo AggregateProgress { get; set; }
      public object Customfield_10200 { get; set; }
      public object Customfield_10201 { get; set; }
      public string Customfield_10400 { get; set; }
      public object Environment { get; set; }
      public object DueDate { get; set; }
      public ProgressInfo Progress { get; set; }
      public VotesInfo Votes { get; set; }
      public Parent Parent { get; set; }
   }

   public class IssueType
   {
      public string Self { get; set; }
      public string Id { get; set; }
      public string Description { get; set; }
      public string IconUrl { get; set; }
      public string Name { get; set; }
      public bool Subtask { get; set; }
   }

   public class ProjectInfo
   {
      public string Self { get; set; }
      public string Id { get; set; }
      public string Key { get; set; }
      public string Name { get; set; }
      public AvatarUrls AvatarUrls { get; set; }
      public Projectcategory ProjectCategory { get; set; }
   }

   public class AvatarUrls
   {
      public string _48x48 { get; set; }
      public string _24x24 { get; set; }
      public string _16x16 { get; set; }
      public string _32x32 { get; set; }
   }

   public class Projectcategory
   {
      public string Self { get; set; }
      public string Id { get; set; }
      public string Description { get; set; }
      public string Name { get; set; }
   }

   public class Resolution
   {
      public string Self { get; set; }
      public string Id { get; set; }
      public string Description { get; set; }
      public string Name { get; set; }
   }

   public class Customfield_10901
   {
      public string Self { get; set; }
      public string Value { get; set; }
      public string Id { get; set; }
   }

   public class Watches
   {
      public string Self { get; set; }
      public int WatchCount { get; set; }
      public bool IsWatching { get; set; }
   }

   public class Priority
   {
      public string Self { get; set; }
      public string IconUrl { get; set; }
      public string Name { get; set; }
      public string Id { get; set; }
   }

   public class Status
   {
      public string Self { get; set; }
      public string Description { get; set; }
      public string IconUrl { get; set; }
      public string Name { get; set; }
      public string Id { get; set; }
      public StatusCategory StatusCategory { get; set; }
   }

   public class StatusCategory
   {
      public string Self { get; set; }
      public int Id { get; set; }
      public string Key { get; set; }
      public string ColorName { get; set; }
      public string Name { get; set; }
   }

   public class UserInfo
   {
      public string Self { get; set; }
      public string Name { get; set; }
      public string Key { get; set; }
      public string EmailAddress { get; set; }
      public AvatarUrls AvatarUrls { get; set; }
      public string DisplayName { get; set; }
      public bool Active { get; set; }
      public string TimeZone { get; set; }
   }

   public class ProgressInfo
   {
      public int Progress { get; set; }
      public int Total { get; set; }
   }

   public class VotesInfo
   {
      public string Self { get; set; }
      public int Votes { get; set; }
      public bool HasVoted { get; set; }
   }

   public class Parent
   {
      public string Id { get; set; }
      public string Key { get; set; }
      public string Self { get; set; }
      public RelatedIssueFields Fields { get; set; }
   }

   public class RelatedIssueFields
   {
      public string Summary { get; set; }
      public Status Status { get; set; }
      public Priority Priority { get; set; }
      public IssueType Issuetype { get; set; }
   }

   public class Issuelink
   {
      public string Id { get; set; }
      public string Self { get; set; }
      public Type Type { get; set; }
      public InwardIssue InwardIssue { get; set; }
      public OutwardIssue OutwardIssue { get; set; }
   }

   public class Type
   {
      public string Id { get; set; }
      public string Name { get; set; }
      public string Inward { get; set; }
      public string Outward { get; set; }
      public string Self { get; set; }
   }

   public class InwardIssue
   {
      public string Id { get; set; }
      public string Key { get; set; }
      public string Self { get; set; }
      public RelatedIssueFields Fields { get; set; }
   }

   public class OutwardIssue
   {
      public string Id { get; set; }
      public string Key { get; set; }
      public string Self { get; set; }
      public RelatedIssueFields Fields { get; set; }
   }

   public class Subtask
   {
      public string Id { get; set; }
      public string Key { get; set; }
      public string Self { get; set; }
      public RelatedIssueFields Fields { get; set; }
   }

   public class SessionInfo
   {
      public string Self { get; set; }
      public string Name { get; set; }
      public LoginInfo LoginInfo { get; set; }
   }

   public class LoginInfo
   {
      public int FailedLoginCount { get; set; }
      public int LoginCount { get; set; }
      public DateTime LastFailedLoginTime { get; set; }
      public DateTime PreviousLoginTime { get; set; }
   }

   public class SuccessfulLoginParameters
   {
      public Session Session { get; set; }
      public Logininfo LoginInfo { get; set; }
   }

   public class Session
   {
      public string Name { get; set; }
      public string Value { get; set; }
   }

   public class Logininfo
   {
      public int FailedLoginCount { get; set; }
      public int LoginCount { get; set; }
      public DateTime LastFailedLoginTime { get; set; }
      public DateTime PreviousLoginTime { get; set; }
   }

}
