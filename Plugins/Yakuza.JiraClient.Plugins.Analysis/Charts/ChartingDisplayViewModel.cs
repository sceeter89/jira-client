using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Yakuza.JiraClient.Api;
using Yakuza.JiraClient.Api.Messages.Actions;
using Yakuza.JiraClient.Api.Messages.IO.Jira;
using Yakuza.JiraClient.Api.Messages.Navigation;
using Yakuza.JiraClient.Api.Model;
using Yakuza.JiraClient.Messaging.Api;

namespace Yakuza.JiraClient.Plugins.Analysis
{
   public class ChartingDisplayViewModel : IMicroservice,
      IHandleMessage<SearchForIssuesResponse>,
      IHandleMessage<CurrentSearchResultsResponse>
   {
      public ICommand ShowEngagementChartCommand { get; private set; }

      public void Handle(CurrentSearchResultsResponse message)
      {
         UpdateData(message.SearchResults);
      }

      public void Handle(SearchForIssuesResponse message)
      {
         UpdateData(message.SearchResults);
      }

      private void UpdateData(IEnumerable<JiraIssue> issues)
      {
         //TODO
      }

      public void Initialize(IMessageBus messageBus)
      {
         messageBus.Register(this);
      }
   }
}
