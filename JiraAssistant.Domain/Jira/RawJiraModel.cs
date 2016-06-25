using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace JiraAssistant.Domain.Jira
{
#pragma warning disable JustCode_CSharp_TypeFileNameMismatch // Types not matching file names
    public class RawSearchResults
    {
        public string Expand { get; set; }
        public long StartAt { get; set; }
        public long MaxResults { get; set; }
        public long Total { get; set; }
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
        public RawChangelog Changelog { get; set; }
        public RawFields BuiltInFields { get; set; }
    }

    public class RawFields
    {
        public RawIssueType IssueType { get; set; }
        public long? TimeSpent { get; set; }
        public RawProjectInfo Project { get; set; }
        public object[] FixVersions { get; set; }
        public long? AggregateTimeSpent { get; set; }
        public RawResolution Resolution { get; set; }
        public DateTime? ResolutionDate { get; set; }
        public long Workratio { get; set; }
        public DateTime? LastViewed { get; set; }
        public RawWatches Watches { get; set; }
        public DateTime Created { get; set; }
        public RawPriority Priority { get; set; }
        public string[] Labels { get; set; }
        public long? TimeEstimate { get; set; }
        public long? AggregateTimeOriginalEstimate { get; set; }
        public object[] Versions { get; set; }
        public RawIssueLink[] Issuelinks { get; set; }
        public RawUserInfo Assignee { get; set; }
        public DateTime Updated { get; set; }
        public RawStatus Status { get; set; }
        public object[] Components { get; set; }
        public long? TimeOriginalEstimate { get; set; }
        public string Description { get; set; }
        public RawTimeTracking TimeTracking { get; set; }
        [JsonProperty("attachment")]
        public RawAttachment[] Attachments { get; set; }
        public long? AggregateTimeEstimate { get; set; }
        public string Summary { get; set; }
        public RawUserInfo Creator { get; set; }
        public RawSubtask[] Subtasks { get; set; }
        public RawUserInfo Reporter { get; set; }
        public RawProgressInfo AggregateProgress { get; set; }
        public string Environment { get; set; }
        public DateTime? DueDate { get; set; }
        public RawProgressInfo Progress { get; set; }
        [JsonProperty("comment")]
        public RawComments Comments { get; set; }
        public RawVotesInfo Votes { get; set; }
        public RawWorklog Worklog { get; set; }
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
        [JsonProperty("48x48")]
        public string Avatar48x48 { get; set; }
        [JsonProperty("24x24")]
        public string Avatar24x24 { get; set; }
        [JsonProperty("16x16")]
        public string Avatar16x16 { get; set; }
        [JsonProperty("32x32")]
        public string Avatar32x32 { get; set; }
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
        public string IconUrl { get; set; }
        public string Name { get; set; }

        public static readonly RawResolution EmptyResolution = new RawResolution
        {
            Self = "N/A",
            Id = "N/A",
            Description = "N /A",
            Name = "N/A",
            IconUrl = "N/A"
        };
    }

    public class RawWatches
    {
        public string Self { get; set; }
        public long WatchCount { get; set; }
        public bool IsWatching { get; set; }
    }

    public class RawPriority
    {
        public string Self { get; set; }
        public string IconUrl { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string StatusColor { get; set; }
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
        public long Id { get; set; }
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
        public long Progress { get; set; }
        public long Total { get; set; }
    }

    public class RawVotesInfo
    {
        public string Self { get; set; }
        public long Votes { get; set; }
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

    public class RawIssueLink
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
        public long FailedLoginCount { get; set; }
        public long LoginCount { get; set; }
        public DateTime LastFailedLoginTime { get; set; }
        public DateTime PreviousLoginTime { get; set; }
    }

    public class RawSuccessfulLoginParameters
    {
        public RawSession Session { get; set; }
        public RawLoginInfo LoginInfo { get; set; }
    }

    public class RawSession
    {
        public string Name { get; set; }
        public string Value { get; set; }
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
        public long CustomId { get; set; }
    }
    
    public class RawProfileDetails
    {
        public string Self { get; set; }
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public RawAvatarUrls AvatarUrls { get; set; }
        public string DisplayName { get; set; }
        public bool Active { get; set; }
        public string TimeZone { get; set; }
        public RawGroups Groups { get; set; }
        public RawApplicationRoles ApplicationRoles { get; set; }
        public string Expand { get; set; }
    }

    public class RawGroups
    {
        public long Size { get; set; }
        public RawGroup[] Items { get; set; }
    }

    public class RawGroup
    {
        public string Name { get; set; }
        public string Self { get; set; }
    }

    public class RawApplicationRoles
    {
        public long Size { get; set; }
        public RawApplicationRole[] Items { get; set; }
    }

    public class RawApplicationRole
    {
        public string Name { get; set; }
        public string Key { get; set; }
    }

    public class RawTimeTracking
    {
    }

    public class RawComments
    {
        public long StartAt { get; set; }
        public long MaxResults { get; set; }
        public long Total { get; set; }
        public RawComment[] Comments { get; set; }
    }

    public class RawComment
    {
        public string Self { get; set; }
        public string Id { get; set; }
        public RawUserInfo Author { get; set; }
        public string Body { get; set; }
        public RawUserInfo UpdateAuthor { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }

    public class RawWorklog
    {
        public long StartAt { get; set; }
        public long MaxResults { get; set; }
        public long Total { get; set; }
        public object[] Worklogs { get; set; }
    }

    public class RawAttachment
    {
        public string Self { get; set; }
        public string Id { get; set; }
        public string Filename { get; set; }
        public RawUserInfo Author { get; set; }
        public DateTime Created { get; set; }
        public long Size { get; set; }
        public string MimeType { get; set; }
        public string Content { get; set; }
        public string Thumbnail { get; set; }
    }

    public class RawFilterDefinition
    {
        public string Self { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public RawUserInfo Owner { get; set; }
        public string Jql { get; set; }
        public string ViewUrl { get; set; }
        public string SearchUrl { get; set; }
        public bool Favourite { get; set; }
    }

    public class RawChangelogItem
    {
        public string Field { get; set; }
        public string Fieldtype { get; set; }
        public string From { get; set; }
        public string FromString { get; set; }
        public string To { get; set; }
        public string toString { get; set; }
    }

    public class RawChangesHistory
    {
        public string Id { get; set; }
        public RawUserInfo Author { get; set; }
        public DateTime Created { get; set; }
        public RawChangelogItem[] Items { get; set; }
    }

    public class RawChangelog
    {
        public int StartAt { get; set; }
        public int MaxResults { get; set; }
        public int Total { get; set; }
        public RawChangesHistory[] Histories { get; set; }
    }
#pragma warning restore JustCode_CSharp_TypeFileNameMismatch // Types not matching file names
}
