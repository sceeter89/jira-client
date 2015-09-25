using System.Collections.Generic;
using Yakuza.JiraClient.Api.Model;
using Yakuza.JiraClient.Messaging.Api;

namespace Yakuza.JiraClient.Api.Messages.IO.Jira
{
   public class SearchForIssuesMessage : IMessage
   {
      public SearchForIssuesMessage(string jqlQuery)
      {
         JqlQuery = jqlQuery;
      }

      public string JqlQuery { get; private set; }
   }

   public class SearchForIssuesResponse : IMessage
   {
      public SearchForIssuesResponse(ICollection<JiraIssue> searchResults)
      {
         SearchResults = searchResults;
      }

      public ICollection<JiraIssue> SearchResults { get; private set; }
   }
}
