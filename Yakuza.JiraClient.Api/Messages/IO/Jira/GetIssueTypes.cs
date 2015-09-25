using System.Collections.Generic;
using Yakuza.JiraClient.Api.Model;
using Yakuza.JiraClient.Messaging.Api;

namespace Yakuza.JiraClient.Api.Messages.IO.Jira
{
   public class GetIssueTypesMessage : IMessage
   {
   }

   public class GetIssueTypesResponse : IMessage
   {
      public GetIssueTypesResponse(IEnumerable<RawIssueType> issueTypes)
      {
         IssueTypes = issueTypes;
      }

      public IEnumerable<RawIssueType> IssueTypes { get; private set; }
   }
}
