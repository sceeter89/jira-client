using System.Collections.Generic;
using Yakuza.JiraClient.Api.Model;
using Yakuza.JiraClient.Messaging.Api;

namespace Yakuza.JiraClient.Api.Messages.IO.Jira
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
