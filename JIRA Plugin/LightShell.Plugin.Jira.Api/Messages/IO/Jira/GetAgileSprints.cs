using System.Collections.Generic;
using LightShell.Plugin.Jira.Api.Model;
using LightShell.Messaging.Api;

namespace LightShell.Plugin.Jira.Api.Messages.IO.Jira
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
