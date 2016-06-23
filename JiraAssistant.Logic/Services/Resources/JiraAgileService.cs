using JiraAssistant.Domain.Exceptions;
using JiraAssistant.Domain.Jira;
using JiraAssistant.Logic.Settings;
using JiraAssistant.Logic.Services.Jira;
using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace JiraAssistant.Logic.Services.Resources
{
    public class JiraAgileService : BaseRestService, IJiraAgileApi
   {
      private readonly IDictionary<int, AgileBoardDataCache> _boardCaches = new Dictionary<int, AgileBoardDataCache>();
      private readonly ApplicationCache _applicationCache;
      private readonly IJiraServerApi _metadataRetriever;
      private readonly IssuesFinder _issuesFinder;

      public JiraAgileService(AssistantSettings configuration, ApplicationCache applicationCache, IJiraServerApi metadataRetriever, IssuesFinder issuesFinder)
         : base(configuration)
      {
         _applicationCache = applicationCache;
         _metadataRetriever = metadataRetriever;
         _issuesFinder = issuesFinder;
      }

      public async Task<RawAgileSprint> GetAgileSprintDetails(int sprintId)
      {
         var client = BuildRestClient();
         var request = new RestRequest("/rest/agile/latest/sprint/{id}", Method.GET);
         request.AddUrlSegment("id", sprintId.ToString());

         var response = await client.ExecuteTaskAsync(request);
         var result = JsonConvert.DeserializeObject<RawAgileSprint>(response.Content);

         return result;
      }

      public async Task<IEnumerable<RawAgileSprint>> GetSprints(int boardId)
      {
         var client = BuildRestClient();
         var request = new RestRequest("/rest/agile/latest/board/{id}/sprint", Method.GET);
         request.AddQueryParameter("maxResults", "500");
         request.AddQueryParameter("startAt", "0");
         request.AddUrlSegment("id", boardId.ToString());

         IRestResponse response;
         RawAgileSprintsList result;
         var allSprints = new List<RawAgileSprint>();
         do
         {
            request.Parameters[1].Value = allSprints.Count;
            response = await client.ExecuteTaskAsync(request);
            result = JsonConvert.DeserializeObject<RawAgileSprintsList>(response.Content);
            if (result.Values == null)
               return allSprints;

            allSprints.AddRange(result.Values);
         } while (result.IsLast == false);

         return allSprints;
      }

      public async Task<IEnumerable<RawAgileEpic>> GetEpics(int boardId)
      {
         var client = BuildRestClient();
         var request = new RestRequest("/rest/agile/latest/board/{id}/epic", Method.GET);
         request.AddQueryParameter("maxResults", "500");
         request.AddQueryParameter("startAt", "0");
         request.AddUrlSegment("id", boardId.ToString());

         IRestResponse response;
         RawAgileEpicsList result;
         var allEpics = new List<RawAgileEpic>();
         do
         {
            request.Parameters[1].Value = allEpics.Count;
            response = await client.ExecuteTaskAsync(request);
            result = JsonConvert.DeserializeObject<RawAgileEpicsList>(response.Content);
            allEpics.AddRange(result.Values);
         } while (result.IsLast == false);

         return allEpics;
      }

      public async Task<IEnumerable<RawAgileBoard>> GetAgileBoards()
      {
         var client = BuildRestClient();
         var request = new RestRequest("/rest/agile/latest/board", Method.GET);
         request.AddQueryParameter("maxResults", "500");
         request.AddQueryParameter("startAt", "0");
         var allBoards = new List<RawAgileBoard>();
         IRestResponse response;
         RawAgileBoardsList result;
         do
         {
            request.Parameters[1].Value = allBoards.Count;
            response = await client.ExecuteTaskAsync(request);
            result = JsonConvert.DeserializeObject<RawAgileBoardsList>(response.Content);
            if (result.Values == null)
            {
               throw new MissingJiraAgileSupportException();
            }
            allBoards.AddRange(result.Values);
         } while (result.IsLast == false);

         return allBoards;
      }

      public async Task<RawAgileBoardConfiguration> GetBoardConfiguration(int boardId)
      {
         var client = BuildRestClient();
         var request = new RestRequest("/rest/agile/latest/board/{id}/configuration", Method.GET);
         request.AddUrlSegment("id", boardId.ToString());

         var response = await client.ExecuteTaskAsync(request);
         var result = JsonConvert.DeserializeObject<RawAgileBoardConfiguration>(response.Content);

         return result;
      }

      private async Task<IList<JiraIssue>> DownloadIssues(int boardId, bool forceReload, Action<float> progressUpdateCallback)
      {
         var boardConfig = await GetBoardConfiguration(boardId);
         var filter = await _metadataRetriever.GetFilterDefinition(boardConfig.Filter.Id);

         if (_boardCaches.ContainsKey(boardId) == false)
            _boardCaches[boardId] = _applicationCache.GetAgileBoardCache(boardId);
         var boardCache = _boardCaches[boardId];

         if (forceReload)
            boardCache.Invalidate();

         var issues = await _issuesFinder.Search(boardCache.PrepareSearchStatement(filter.Jql), progressUpdateCallback);

         issues = await boardCache.UpdateCache(issues);

         return issues;
      }

      private async Task<IList<RawAgileEpic>> DownloadEpics(int boardId)
      {
         var epics = await GetEpics(boardId);

         return epics.ToList();
      }

      private async Task<IList<RawAgileSprint>> DownloadSprints(int boardId)
      {
         var sprints = await GetSprints(boardId);

         return sprints.ToList();
      }

      public async Task<AgileBoardIssues> GetBoardContent(int boardId, bool forceReload = false, Action<float> progressUpdateCallback = null)
      {
         var sprintsTask = DownloadSprints(boardId);
         var epicsTask = DownloadEpics(boardId);
         var issuesTask = DownloadIssues(boardId, forceReload, progressUpdateCallback);

         await Task.Factory.StartNew(() => Task.WaitAll(sprintsTask, epicsTask, issuesTask));

         var boardData = await Task.Factory.StartNew(() => new AgileBoardIssues(issuesTask.Result, epicsTask.Result, sprintsTask.Result));

         return boardData;
      }
   }
}
