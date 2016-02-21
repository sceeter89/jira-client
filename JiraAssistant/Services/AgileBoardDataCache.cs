using JiraAssistant.Model;
using JiraAssistant.Model.Jira;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Threading.Tasks;

namespace JiraAssistant.Services
{
   public class AgileBoardDataCache
   {
      private const string MetaFileName = ".metafile";
      private readonly DateTime _initialDateTime = new DateTime(1970, 1, 1);

      private readonly int _boardId;
      private readonly string _directoryName;
      private AgileBoardCacheMetadata _metadata;
      private readonly IsolatedStorageFile _storage;
      private readonly string _jiraUrl;

      public AgileBoardDataCache(string baseCacheDirectory, int boardId, string jiraUrl)
      {
         _boardId = boardId;
         _jiraUrl = jiraUrl;
         _directoryName = Path.Combine(baseCacheDirectory, "AgileBoards", boardId.ToString());

         _storage = IsolatedStorageFile.GetMachineStoreForAssembly();

         FetchCacheInformation();
      }

      public bool IsAvailable { get; private set; }

      private void FetchCacheInformation()
      {
         if (_storage.DirectoryExists(_directoryName) == false)
         {
            IsAvailable = false;
            return;
         }

         var metaFilePath = Path.Combine(_directoryName, MetaFileName);
         if (_storage.FileExists(metaFilePath) == false)
         {
            IsAvailable = false;
            return;
         }

         try
         {
            using (var stream = _storage.OpenFile(metaFilePath, FileMode.Open))
            using (var reader = new StreamReader(stream))
            {
               _metadata = JsonConvert.DeserializeObject<AgileBoardCacheMetadata>(reader.ReadToEnd());
               IsAvailable = true;
            }
         }
         catch
         {
            IsAvailable = false;
            return;
         }
      }

      public async Task<IEnumerable<JiraIssue>> UpdateCache(IEnumerable<JiraIssue> updatedIssues)
      {
         if (IsAvailable == false)
            InitializeCacheDirectory();

         var cachedItems = await LoadIssuesFromCache();

         var updatedCache = await Task.Factory.StartNew(() => updatedIssues.Union(cachedItems));

         await DumpCache(updatedCache);
         await StoreMetafile();

         return updatedCache;
      }

      private async Task StoreMetafile()
      {
         var metadata = new AgileBoardCacheMetadata { DownloadedTime = DateTime.Now };

         var metaFilePath = Path.Combine(_directoryName, MetaFileName);

         using (var stream = _storage.OpenFile(metaFilePath, FileMode.Create))
         using (var writer = new StreamWriter(stream))
         {
            await writer.WriteAsync(JsonConvert.SerializeObject(metadata));
         }
      }

      private async Task DumpCache(IEnumerable<JiraIssue> updatedCache)
      {
         if (_storage.DirectoryExists(_directoryName) == false)
            _storage.CreateDirectory(_directoryName);

         var issuesCachePath = Path.Combine(_directoryName, "issues.db");

         await Task.Factory.StartNew(async () =>
         {
            using (var stream = _storage.OpenFile(issuesCachePath, FileMode.Create))
            using (var writer = new StreamWriter(stream))
            {
               foreach (var issue in updatedCache)
               {
                  var line = await Task.Factory.StartNew(() => JsonConvert.SerializeObject(issue));
                  await writer.WriteLineAsync(line);
               }
            }
         });
      }

      private async Task<IEnumerable<JiraIssue>> LoadIssuesFromCache()
      {
         var issuesCachePath = Path.Combine(_directoryName, "issues.db");

         var result = new List<JiraIssue>();

         if (_storage.FileExists(issuesCachePath) == false)
            return result;

         using (var stream = _storage.OpenFile(issuesCachePath, FileMode.Open))
         using (var reader = new StreamReader(stream))
         {
            string line = null;
            while ((line = await reader.ReadLineAsync()) != null)
            {
               var issue = await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<JiraIssue>(line));
               result.Add(issue);
            }
         }

         return result;
      }

      public string PrepareSearchStatement(string originalJql)
      {
         if (IsAvailable == false)
            return originalJql;

         return string.Format("updated >= '{1:yyyy-MM-dd hh:mm}' AND {0}", originalJql, _metadata.DownloadedTime);
      }

      private void InitializeCacheDirectory()
      {
         _storage.CreateDirectory(_directoryName);
         StoreMetafile();
      }
   }
}
