using JiraAssistant.Domain;
using JiraAssistant.Domain.Exceptions;
using JiraAssistant.Domain.Jira;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace JiraAssistant.Logic.Services
{
    public class AgileBoardDataCache
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private const string MetaFileName = ".metafile";
        private const string IssuesCacheFileName = "issues.db";
        private readonly DateTime _initialDateTime = new DateTime(1970, 1, 1);

        private readonly int _boardId;
        private AgileBoardCacheMetadata _metadata;
        private readonly string _cachePath;
        private readonly string _jiraUrl;

        public AgileBoardDataCache(string baseCacheDirectory, int boardId, string jiraUrl)
        {
            _boardId = boardId;
            _jiraUrl = jiraUrl;
            _cachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                                                              "Yakuza", "Jira Assistant", "Cache", "AgileBoards", boardId.ToString());

            FetchCacheInformation();
        }

        public bool IsAvailable { get; private set; }

        private void FetchCacheInformation()
        {
            if (Directory.Exists(_cachePath) == false)
            {
                IsAvailable = false;
                return;
            }

            var metaFilePath = Path.Combine(_cachePath, MetaFileName);
            if (File.Exists(metaFilePath) == false)
            {
                IsAvailable = false;
                return;
            }

            try
            {
                using (var reader = new StreamReader(metaFilePath))
                {
                    _metadata = JsonConvert.DeserializeObject<AgileBoardCacheMetadata>(reader.ReadToEnd());

                    if (_metadata.ModelVersion != JiraIssue.ModelVersion)
                    {
                        IsAvailable = false;
                        return;
                    }

                    IsAvailable = true;
                }
            }
            catch (Exception e)
            {
                _logger.Info(e, "Invalidating cache because it failed to load.");
                IsAvailable = false;
                return;
            }
        }

        public async Task<IList<JiraIssue>> UpdateCache(IList<JiraIssue> updatedIssues)
        {
            if (_jiraUrl == "demo")
                return updatedIssues;

            try
            {
                if (IsAvailable == false)
                    InitializeCacheDirectory();

                var cachedItems = await LoadIssuesFromCache();

                var updatedCache = (await Task.Factory.StartNew(() => updatedIssues.Union(cachedItems))).ToList();

                await DumpCache(updatedCache);
                await StoreMetafile();

                return updatedCache;
            }
            catch (Exception e)
            {
                _logger.Warn(e, "Cache update failed");
                throw new CacheCorruptedException();
            }
        }

        private async Task StoreMetafile()
        {
            var metadata = new AgileBoardCacheMetadata
            {
                DownloadedTime = DateTime.Now,
                GeneratorVersion = GetType().Assembly.GetName().Version,
                ModelVersion = JiraIssue.ModelVersion
            };

            var metaFilePath = Path.Combine(_cachePath, MetaFileName);

            using (var writer = new StreamWriter(metaFilePath))
            {
                await writer.WriteAsync(JsonConvert.SerializeObject(metadata));
            }
        }

        private async Task DumpCache(IEnumerable<JiraIssue> updatedCache)
        {
            if (Directory.Exists(_cachePath) == false)
                Directory.CreateDirectory(_cachePath);

            var issuesCachePath = Path.Combine(_cachePath, IssuesCacheFileName);

            await Task.Factory.StartNew(async () =>
            {
                using (var writer = new StreamWriter(issuesCachePath))
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
            var issuesCachePath = Path.Combine(_cachePath, IssuesCacheFileName);

            var result = new List<JiraIssue>();

            if (File.Exists(issuesCachePath) == false)
                return result;

            using (var reader = new StreamReader(issuesCachePath))
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

        private async void InitializeCacheDirectory()
        {
            if (Directory.Exists(_cachePath))
                Directory.Delete(_cachePath, recursive: true);

            Directory.CreateDirectory(_cachePath);

            await StoreMetafile();
        }

        public void Invalidate()
        {
            Directory.Delete(_cachePath, recursive: true);
            FetchCacheInformation();
        }
    }
}
