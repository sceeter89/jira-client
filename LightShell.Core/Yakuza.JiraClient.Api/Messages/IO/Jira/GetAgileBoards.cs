using System.Collections.Generic;
using Yakuza.JiraClient.Api.Model;
using Yakuza.JiraClient.Messaging.Api;

namespace Yakuza.JiraClient.Api.Messages.IO.Jira
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
