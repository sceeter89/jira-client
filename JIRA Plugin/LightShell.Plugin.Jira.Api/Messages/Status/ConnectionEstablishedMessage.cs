using LightShell.Plugin.Jira.Api.Model;
using LightShell.Messaging.Api;

namespace LightShell.Plugin.Jira.Api.Messages.Status
{
   public class ConnectionEstablishedMessage : IMessage
   {
      public ConnectionEstablishedMessage(RawProfileDetails profile)
      {
         Profile = profile;
      }
      
      public RawProfileDetails Profile { get; private set; }
   }
}
