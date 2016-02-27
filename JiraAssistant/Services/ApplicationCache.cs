using JiraAssistant.Services.Settings;
using System.IO;

namespace JiraAssistant.Services
{
   public class ApplicationCache
   {
      private readonly AssistantSettings _configuration;
      private readonly string _baseCacheDirectory;

      public ApplicationCache(AssistantSettings configuration)
      {
         _configuration = configuration;

         _baseCacheDirectory = Path.Combine("Cache", configuration.JiraUrl.GetHashCode().ToString());
      }

      public AgileBoardDataCache GetAgileBoardCache(int boardId)
      {
         return new AgileBoardDataCache(_baseCacheDirectory, boardId, _configuration.JiraUrl);
      }
   }
}
