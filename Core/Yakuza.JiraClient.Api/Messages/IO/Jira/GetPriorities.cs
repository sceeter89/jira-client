using System.Collections.Generic;
using Yakuza.JiraClient.Api.Model;
using Yakuza.JiraClient.Messaging.Api;

namespace Yakuza.JiraClient.Api.Messages.IO.Jira
{
   public class GetPrioritiesMessage : IMessage
   {
   }

   public class GetPrioritiesResponse : IMessage
   {
      public GetPrioritiesResponse(IEnumerable<RawPriority> priorities)
      {
         Priorities = priorities;
      }

      public IEnumerable<RawPriority> Priorities { get; private set; }
   }
}
