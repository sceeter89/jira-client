using System.Collections.Generic;
using Yakuza.JiraClient.Api.Model;
using Yakuza.JiraClient.Messaging.Api;

namespace Yakuza.JiraClient.Api.Messages.Actions
{
   public class CurrentSearchResultsMessage : IMessage
   {
   }

   public class CurrentSearchResultsResponse : IMessage
   {
      public CurrentSearchResultsResponse(ICollection<JiraIssue> searchResults)
      {
         SearchResults = searchResults;
      }

      public ICollection<JiraIssue> SearchResults { get; private set; }
   }
}
