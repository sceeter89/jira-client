using Newtonsoft.Json;
using System;

namespace JiraAssistant.Model.Github
{
   public class GithubReleaseArtifact
   {
      public string Name { get; set; }
      public int Size { get; set; }
      [JsonProperty("created_at")]
      public DateTime CreatedAt { get; set; }
      [JsonProperty("browser_download_url")]
      public string BrowserDownloadUrl { get; set; }
   }
}