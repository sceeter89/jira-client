using System.Collections.Generic;
using LightShell.Plugin.Jira.Api.Model;
using LightShell.Messaging.Api;

namespace LightShell.Plugin.Jira.Api.Messages.IO.Jira
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
