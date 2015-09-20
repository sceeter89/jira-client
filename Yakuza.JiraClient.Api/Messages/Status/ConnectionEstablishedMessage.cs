using Yakuza.JiraClient.Api.Model;

namespace Yakuza.JiraClient.Api.Messages.Status
{
   public class ConnectionEstablishedMessage
   {
      public ConnectionEstablishedMessage(RawProfileDetails profile)
      {
         Profile = profile;
      }
      
      public RawProfileDetails Profile { get; private set; }
   }
}
