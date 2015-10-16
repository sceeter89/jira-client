using System.Collections.Generic;
using Yakuza.JiraClient.Api.Model;
using Yakuza.JiraClient.Messaging.Api;

namespace Yakuza.JiraClient.Api.Messages.IO.Jira
{
   public class GetAgileSprintsMessage : IMessage
   {
      public GetAgileSprintsMessage(RawAgileBoard board)
      {
         Board = board;
      }

      public RawAgileBoard Board { get; private set; }
   }

   public class GetAgileSprintsResponse : IMessage
   {
      public GetAgileSprintsResponse(RawAgileBoard board, IEnumerable<RawAgileSprint> sprints)
      {
         Board = board;
         Sprints = sprints;
      }

      public RawAgileBoard Board { get; private set; }
      public IEnumerable<RawAgileSprint> Sprints { get; private set; }
   }
}
