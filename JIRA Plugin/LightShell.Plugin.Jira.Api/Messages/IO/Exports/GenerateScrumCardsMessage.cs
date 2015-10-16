using System.Collections.Generic;
using LightShell.Plugin.Jira.Api.Model;
using LightShell.Messaging.Api;

namespace LightShell.Plugin.Jira.Api.Messages.IO.Exports
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
