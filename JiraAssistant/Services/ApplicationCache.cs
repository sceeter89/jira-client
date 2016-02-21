using JiraAssistant.Model;
using System.IO;

namespace JiraAssistant.Services
{
   public class ApplicationCache
   {
      private readonly AssistantConfiguration _configuration;
      private readonly string _baseCacheDirectory;

      public ApplicationCache(AssistantConfiguration configuration)
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
