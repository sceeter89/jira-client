using System.Collections.Generic;
using LightShell.Plugin.Jira.Api.Model;
using LightShell.Messaging.Api;

namespace LightShell.Plugin.Jira.Api.Messages.IO.Jira
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
