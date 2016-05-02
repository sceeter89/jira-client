using JiraAssistant.Model.Jira;
using JiraAssistant.Services.Jira;
using JiraAssistant.Services.Resources;
using JiraAssistant.Services.Settings;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Media;
using System;

namespace JiraAssistant.Services
{
   public class JiraApi : IJiraApi
   {
      private readonly IssuesFinder _issuesFinder;
      private readonly ResourceDownloader _resourceDownloader;

      public JiraApi(AssistantSettings configuration)
      {
         Session = new JiraSessionService(configuration);
         Agile = new JiraAgileService(configuration);
         Server = new MetadataRetriever(configuration);
         Worklog = new WorklogManager(configuration);

         _resourceDownloader = new ResourceDownloader(configuration);
         _issuesFinder = new IssuesFinder(configuration, Server);
      }

      public IJiraAgileApi Agile
      {
         get; private set;
      }

      public IJiraServerApi Server
      {
         get; private set;
      }

      public IJiraSessionApi Session
      {
         get; private set;
      }

      public IJiraWorklogManager Worklog
      {
         get; private set;
      }

      public async Task<ImageSource> DownloadPicture(string imageUri)
      {
         return await _resourceDownloader.DownloadPicture(imageUri);
      }

      public async Task<IEnumerable<JiraIssue>> SearchForIssues(string jqlQuery)
      {
         return await _issuesFinder.Search(jqlQuery);
      }
   }
}
