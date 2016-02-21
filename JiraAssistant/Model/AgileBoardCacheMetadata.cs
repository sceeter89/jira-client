using System;

namespace JiraAssistant.Model
{
   public class AgileBoardCacheMetadata
   {
      public DateTime DownloadedTime { get; set; }
      public Version GeneratorVersion { get; set; }
      public Version ModelVersion { get; set; }
   }
}
