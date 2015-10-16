using System.Collections.Generic;
using LightShell.Plugin.Jira.Api.Model;
using LightShell.Messaging.Api;

namespace LightShell.Plugin.Jira.Api.Messages.IO.Jira
{
   public class GetResolutionsMessage : IMessage
   {
   }

   public class GetResolutionsResponse : IMessage
   {
      public GetResolutionsResponse(IEnumerable<RawResolution> resolutions)
      {
         Resolutions = resolutions;
      }

      public IEnumerable<RawResolution> Resolutions { get; private set; }
   }
}
