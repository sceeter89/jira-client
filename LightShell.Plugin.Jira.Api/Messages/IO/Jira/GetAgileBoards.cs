using System.Collections.Generic;
using LightShell.Plugin.Jira.Api.Model;
using LightShell.Messaging.Api;

namespace LightShell.Plugin.Jira.Api.Messages.IO.Jira
{
   public class GetAgileBoardsMessage : IMessage
   {
   }

   public class GetAgileBoardsResponse : IMessage
   {
      public GetAgileBoardsResponse(IEnumerable<RawAgileBoard> boards)
      {
         Boards = boards;
      }

      public IEnumerable<RawAgileBoard> Boards { get; private set; }
   }

   public class JiraAgileSupportMissing : IMessage
   {
   }
}
