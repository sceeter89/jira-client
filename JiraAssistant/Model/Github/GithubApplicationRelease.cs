using Newtonsoft.Json;
using System;

namespace JiraAssistant.Model.Github
{
   public class GithubApplicationRelease
   {
      [JsonProperty("html_url")]
      public string HtmlUrl { get; set; }
      [JsonProperty("tag_name")]
      public string TagName { get; set; }
      public string Name { get; set; }
      public bool Draft { get; set; }
      public bool Prerelease { get; set; }
      [JsonProperty("published_at")]
      public DateTime PublishedAt { get; set; }
      public GithubReleaseArtifact[] Assets { get; set; }
      public string Body { get; set; }
   }
}
