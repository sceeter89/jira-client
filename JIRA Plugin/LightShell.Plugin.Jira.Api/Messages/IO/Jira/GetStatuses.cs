using System.Collections.Generic;
using LightShell.Messaging.Api;
using LightShell.Plugin.Jira.Api.Model;

namespace LightShell.Plugin.Jira.Api.Messages.IO.Jira
{
   public class GetStatusesMessage : IMessage
   {
   }

   public class GetStatusesResponse : IMessage
   {
      public GetStatusesResponse(IEnumerable<RawStatus> statuses)
      {
         Statuses = statuses;
      }

      public IEnumerable<RawStatus> Statuses { get; private set; }
   }
}
