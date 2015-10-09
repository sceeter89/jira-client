using Yakuza.JiraClient.Api.Model;
using Yakuza.JiraClient.Messaging.Api;

namespace Yakuza.JiraClient.Api.Messages.Status
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
