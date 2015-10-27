using System.Collections.Generic;
using LightShell.Plugin.Jira.Api.Model;
using LightShell.Messaging.Api;

namespace LightShell.Plugin.Jira.Api.Messages.IO.Jira
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
