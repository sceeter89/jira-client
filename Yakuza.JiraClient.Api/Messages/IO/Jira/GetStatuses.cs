using System.Collections.Generic;
using Yakuza.JiraClient.Api.Model;
using Yakuza.JiraClient.Messaging.Api;

namespace Yakuza.JiraClient.Api.Messages.IO.Jira
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
