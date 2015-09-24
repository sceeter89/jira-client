using System.Collections.Generic;
using Yakuza.JiraClient.Api.Model;
using Yakuza.JiraClient.Messaging.Api;

namespace Yakuza.JiraClient.Api.Messages.Actions
{
   public class NewSearchResultsAvailableMessage : IMessage
   {
      public NewSearchResultsAvailableMessage(IList<JiraIssue> searchResults)
      {
         SearchResults = searchResults;
      }

      public IList<JiraIssue> SearchResults { get; private set; }
   }
}
