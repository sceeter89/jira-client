using Yakuza.JiraClient.Api.Model;
using Yakuza.JiraClient.Messaging.Api;

namespace Yakuza.JiraClient.Api.Messages.IO.Jira
{
   public class GetProfileDetailsMessage : IMessage
   {
   }

   public class GetProfileDetailsResponse : IMessage
   {
      public GetProfileDetailsResponse(RawProfileDetails details)
      {
         Details = details;
      }

      public RawProfileDetails Details { get; private set; }
   }
}
