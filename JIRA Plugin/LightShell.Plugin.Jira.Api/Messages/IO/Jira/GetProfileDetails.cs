using LightShell.Plugin.Jira.Api.Model;
using LightShell.Messaging.Api;

namespace LightShell.Plugin.Jira.Api.Messages.IO.Jira
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
