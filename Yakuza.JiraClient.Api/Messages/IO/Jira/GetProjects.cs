using System.Collections.Generic;
using Yakuza.JiraClient.Api.Model;
using Yakuza.JiraClient.Messaging.Api;

namespace Yakuza.JiraClient.Api.Messages.IO.Jira
{
   public class GetProjectsMessage : IMessage
   {
   }

   public class GetProjectsResponse : IMessage
   {
      public GetProjectsResponse(IEnumerable<RawProjectInfo> projects)
      {
         Projects = projects;
      }

      public IEnumerable<RawProjectInfo> Projects { get; private set; }
   }
}
