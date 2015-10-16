using System.Collections.Generic;
using LightShell.Plugin.Jira.Api.Model;
using LightShell.Messaging.Api;

namespace LightShell.Plugin.Jira.Api.Messages.IO.Jira
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

   public class SearchFailedResponse : IMessage
   {
      public FailureReason Reason { get; private set; }

      public enum FailureReason { NoResultsYielded, ExceptionOccured }

      public SearchFailedResponse(FailureReason reason)
      {
         Reason = reason;
      }
   }
}
