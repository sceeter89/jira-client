﻿using JiraAssistant.Domain.Jira;
using JiraAssistant.Logic.Settings;
using JiraAssistant.Logic.Services.Resources;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Drawing;

namespace JiraAssistant.Logic.Services
{
    public class JiraApi : IJiraApi
   {
      private readonly IssuesFinder _issuesFinder;
      private readonly ResourceDownloader _resourceDownloader;

      public JiraApi(AssistantSettings configuration, ApplicationCache applicationCache)
      {
         _resourceDownloader = new ResourceDownloader(configuration);

         Session = new JiraSessionService(configuration);
         Server = new MetadataRetriever(configuration);
         _issuesFinder = new IssuesFinder(configuration, Server);
         Agile = new JiraAgileService(configuration, applicationCache, Server, _issuesFinder);
         Worklog = new WorklogManager(configuration);
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

      public async Task<Bitmap> DownloadPicture(string imageUri)
      {
         return await _resourceDownloader.DownloadPicture(imageUri);
      }

      public async Task<IList<JiraIssue>> SearchForIssues(string jqlQuery)
      {
         return await _issuesFinder.Search(jqlQuery);
      }
   }
}
