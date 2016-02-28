using System;

namespace JiraAssistant.Model.Github
{
   public class GithubReleaseArtifact
   {
      public string Name { get; set; }
      public int Size { get; set; }
      public DateTime CreatedAt { get; set; }
      public string BrowserDownloadUrl { get; set; }
   }
}