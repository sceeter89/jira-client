using System.Collections.Generic;
using Yakuza.JiraClient.Api.Model;
using Yakuza.JiraClient.Messaging.Api;

namespace Yakuza.JiraClient.Api.Messages.IO.Jira
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
