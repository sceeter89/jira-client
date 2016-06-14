using System;

namespace JiraAssistant.Domain
{
   public class AgileBoardCacheMetadata
   {
      public DateTime DownloadedTime { get; set; }
      public Version GeneratorVersion { get; set; }
      public Version ModelVersion { get; set; }
   }
}
