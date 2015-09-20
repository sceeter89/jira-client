using System.Collections.Generic;
using Yakuza.JiraClient.Api.Model;

namespace Yakuza.JiraClient.Api.Messages.Actions
{
   public class NewSearchResultsAvailable
   {
      public NewSearchResultsAvailable(IList<JiraIssue> searchResults)
      {
         SearchResults = searchResults;
      }

      public IList<JiraIssue> SearchResults { get; private set; }
   }
}
