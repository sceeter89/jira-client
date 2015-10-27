using System.Collections.Generic;
using LightShell.Plugin.Jira.Api.Model;
using LightShell.Messaging.Api;

namespace LightShell.Plugin.Jira.Api.Messages.Actions
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
