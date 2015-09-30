﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Yakuza.JiraClient.Api;
using Yakuza.JiraClient.Api.Messages.IO.Jira;
using Yakuza.JiraClient.Api.Model;
using Yakuza.JiraClient.Messaging.Api;

namespace Yakuza.JiraClient.IO.Jira
{
   public class IssuesSearchMicroservice : RestMicroserviceBase,
      IMicroservice,
      IHandleMessage<SearchForIssuesMessage>,
      IHandleMessage<GetFieldsDescriptionsResponse>
   {
      private IEnumerable<RawFieldDefinition> _descriptions;
      private readonly IList<RawIssue> _searchResult = new List<RawIssue>();
      private IDictionary<string, RawFieldDefinition> _fields;

      public IssuesSearchMicroservice(IConfiguration configuration, IMessageBus messageBus)
         : base(configuration, messageBus)
      {
         _messageBus.Register(this);
      }

      public void Handle(GetFieldsDescriptionsResponse message)
      {
         _descriptions = message.Descriptions;
         _fields = _descriptions.ToDictionary(d => d.Name, d => d);

         if (_searchResult.Any())
         {
            _messageBus.Send(new SearchForIssuesResponse(ConvertIssuesToDomainModel()));
            _searchResult.Clear();
         }
      }

      public async void Handle(SearchForIssuesMessage message)
      {
         var client = BuildRestClient();
         var request = new RestRequest("/rest/api/latest/search", Method.POST);
         _searchResult.Clear();
         do
         {
            request.AddJsonBody(new
            {
               jql = message.JqlQuery,
               startAt = 0,
               maxResults = 500
            });
            var response = await client.ExecuteTaskAsync(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
               _messageBus.LogMessage(LogLevel.Fatal, "Search request failed with invalid response code: {0}.\r\nResponse content is: {1}", response.StatusCode, response.Content);
               _messageBus.Send(new SearchFailedResponse(SearchFailedResponse.FailureReason.ExceptionOccured));
            }
            var searchResults = await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<RawSearchResults>(response.Content));
            foreach (var issue in searchResults.Issues)
            {
               _searchResult.Add(issue);
            }
            if (_searchResult.Count >= searchResults.Total)
               break;
         } while (true);

         if (_searchResult.Any() == false)
         {
            _messageBus.Send(new SearchFailedResponse(SearchFailedResponse.FailureReason.NoResultsYielded));
            return;
         }

         if (_fields == null)
         {
            _messageBus.Send(new GetFieldsDescriptionsMessage());
            return;
         }

         _messageBus.Send(new SearchForIssuesResponse(ConvertIssuesToDomainModel()));
         _searchResult.Clear();
      }

      private ICollection<JiraIssue> ConvertIssuesToDomainModel()
      {
         return _searchResult.Select(Convert).ToList();
      }

      private JiraIssue Convert(RawIssue issue)
      {
         return new JiraIssue
         {
            Key = issue.Key,
            Project = issue.BuiltInFields.Project.Name,
            Summary = issue.BuiltInFields.Summary,
            Priority = issue.BuiltInFields.Priority.Name,
            StoryPoints = (int)(GetFieldByName<float?>(issue, "Story Points") ?? 0),
            Subtasks = issue.BuiltInFields.Subtasks.Count(),
            Created = issue.BuiltInFields.Created,
            Resolved = issue.BuiltInFields.ResolutionDate ?? DateTime.MinValue,
            Status = issue.BuiltInFields.Status.Name,
            Description = issue.BuiltInFields.Description,
            Assignee = (issue.BuiltInFields.Assignee ?? RawUserInfo.EmptyInfo).DisplayName,
            Reporter = (issue.BuiltInFields.Reporter ?? RawUserInfo.EmptyInfo).DisplayName,
            BuiltInFields = issue.BuiltInFields
         };
      }

      private T GetFieldByName<T>(RawIssue issue, string fieldName, string path = null)
      {
         if (_fields.ContainsKey(fieldName) == false)
            return default(T);

         var fieldId = _fields[fieldName].Id;
         JToken token;
         if (path == null)
            return issue.RawFields.Value<T>(fieldId);
         else
         {
            token = issue.RawFields.SelectToken(fieldId);
            foreach (var part in path.Split('/'))
            {
               token = token.SelectToken(part);
            }
         }

         if (token == null)
            return default(T);

         return token.ToObject<T>();
      }
   }
}