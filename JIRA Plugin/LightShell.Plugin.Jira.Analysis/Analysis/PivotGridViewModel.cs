using GalaSoft.MvvmLight;
using Telerik.Pivot.Core;
using System.Linq;
using GalaSoft.MvvmLight.Threading;
using System.Collections.Generic;
using LightShell.Api;
using LightShell.Messaging.Api;
using LightShell.Plugin.Jira.Api.Messages.IO.Jira;
using LightShell.Plugin.Jira.Api.Messages.Actions;
using LightShell.Plugin.Jira.Api.Model;
using System;
using LightShell.Plugin.Jira.Api;
using System.Windows.Input;
using LightShell.Api.Messages.Navigation;

namespace LightShell.Plugin.Jira.Analysis.Analysis
{
   public class PivotGridViewModel : ViewModelBase,
      IMicroservice,
      IHandleMessage<SearchForIssuesResponse>,
      IHandleMessage<CurrentSearchResultsResponse>
   {
      private IMessageBus _messageBus;

      public PivotGridViewModel()
      {
         DispatcherHelper.CheckBeginInvokeOnUI(() =>
         DataSource = new LocalDataSourceProvider());
      }

      public void Handle(SearchForIssuesResponse message)
      {
         LoadSearchResults(message.SearchResults);
      }

      public void Handle(CurrentSearchResultsResponse message)
      {
         LoadSearchResults(message.SearchResults);
      }

      private void LoadSearchResults(IEnumerable<JiraIssue> searchResults)
      {
         DispatcherHelper.CheckBeginInvokeOnUI(() =>
         {
            var list = searchResults.Select(x => new PivotJiraIssue(x)).ToList();

            using (DataSource.DeferRefresh())
            {

               DataSource.ItemsSource = list;

            }
         });
      }

      public void Initialize(IMessageBus messageBus)
      {
         _messageBus = messageBus;
         _messageBus.Register(this);

         _messageBus.Send(new CurrentSearchResultsMessage());

         OpenWindowCommand = new LoginEnabledRelayCommand(
            ()=> _messageBus.Send(new ShowDocumentPaneMessage(this, "Pivot analysis",
                                                   new PivotReportingGrid { DataContext = this },
                                                   new PivotReportingProperties { DataContext = this })), messageBus);
      }

      public LocalDataSourceProvider DataSource { get; private set; }
      public ICommand OpenWindowCommand { get; private set; }
   }
}
