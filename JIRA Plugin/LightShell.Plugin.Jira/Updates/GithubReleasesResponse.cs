using System;

namespace Yakuza.JiraClient.IO.Updates
{
   public class GithubApplicationRelease
   {
      public string html_url { get; set; }
      public string tag_name { get; set; }
      public string name { get; set; }
      public bool draft { get; set; }
      public bool prerelease { get; set; }
      public DateTime published_at { get; set; }
      public GithubReleaseArtifact[] assets { get; set; }
      public string body { get; set; }
   }

   public class GithubReleaseArtifact
   {
      public string name { get; set; }
      public int size { get; set; }
      public DateTime created_at { get; set; }
      public string browser_download_url { get; set; }
   }
}
