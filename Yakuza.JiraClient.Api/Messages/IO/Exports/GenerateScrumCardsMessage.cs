using System.Collections.Generic;
using Yakuza.JiraClient.Api.Model;
using Yakuza.JiraClient.Messaging.Api;

namespace Yakuza.JiraClient.Api.Messages.IO.Exports
{
   public class GenerateScrumCardsMessage : IMessage
   {
      public enum ExportFileFormat { Xps }

      public GenerateScrumCardsMessage(IEnumerable<JiraIssue> issues, ExportFileFormat format = ExportFileFormat.Xps)
      {
         Issues = issues;
         Format = format;
      }

      public IEnumerable<JiraIssue> Issues { get; private set; }
      public ExportFileFormat Format { get; private set; }
   }
}
