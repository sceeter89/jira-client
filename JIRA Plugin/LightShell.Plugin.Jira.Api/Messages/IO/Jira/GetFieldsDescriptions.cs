using System.Collections.Generic;
using LightShell.Plugin.Jira.Api.Model;
using LightShell.Messaging.Api;

namespace LightShell.Plugin.Jira.Api.Messages.IO.Jira
{
   public class GetFieldsDescriptionsMessage : IMessage
   {
   }

   public class GetFieldsDescriptionsResponse : IMessage
   {
      public GetFieldsDescriptionsResponse(IEnumerable<RawFieldDefinition> descriptions)
      {
         Descriptions = descriptions;
      }

      public IEnumerable<RawFieldDefinition> Descriptions { get; private set; }
   }
}
