using System;

namespace JiraAssistant.Model.Github
{
   public class GithubApplicationRelease
   {
      public string HtmlUrl { get; set; }
      public string TagName { get; set; }
      public string Name { get; set; }
      public bool Draft { get; set; }
      public bool Prerelease { get; set; }
      public DateTime PublishedAt { get; set; }
      public GithubReleaseArtifact[] Assets { get; set; }
      public string Body { get; set; }
   }
}
