using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using LightShell.Api;
using LightShell.Api.Messages.Navigation;
using LightShell.Messaging.Api;
using LightShell.Plugin.Jira.Api.Messages.Actions;
using LightShell.Plugin.Jira.Api.Messages.Actions.Authentication;
using LightShell.Plugin.Jira.Api.Messages.IO.Jira;
using LightShell.Plugin.Jira.Api.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Yakuza.JiraClient.Plugins.Analysis.Charts;

namespace LightShell.Plugin.Jira.Analysis.Charts
{
   public class EngagementChartViewModel : IMicroservice,
      IHandleMessage<LoggedInMessage>,
      IHandleMessage<LoggedOutMessage>,
      IHandleMessage<SearchForIssuesResponse>,
      IHandleMessage<CurrentSearchResultsResponse>
   {
      public EngagementChartViewModel()
      {
         DispatcherHelper.CheckBeginInvokeOnUI(() =>
         {
            Items = new ObservableCollection<EngagementItem>();

            AvailableCriterias = new[]
            {
               new EngagementCriteria("Total story points",
                  issues => issues.Sum(i => i.StoryPoints)),
               new EngagementCriteria("Total issues",
                  issues => issues.Count())
            };
            SelectedCriteria = AvailableCriterias[0];

            AvailableBases = new[]
            {
              new EngagementBase("Assignee", issue => issue.Assignee ?? ""),
              new EngagementBase("Reporter", issue => issue.Reporter ?? "")
            };
            SelectedBase = AvailableBases[0];
         });
      }

      public void Handle(CurrentSearchResultsResponse message)
      {
         UpdateData(message.SearchResults);
      }

      public void Handle(SearchForIssuesResponse message)
      {
         UpdateData(message.SearchResults);
      }

      private IEnumerable<JiraIssue> _issuesOnChart;
      private EngagementCriteria _selectedCriteria;
      private EngagementBase _selectedBase;
      private bool _isLoggedIn;

      public RelayCommand OpenWindowCommand { get; private set; }

      private void UpdateData(IEnumerable<JiraIssue> issues)
      {
         if (Items == null || issues.Any() == false) return;

         _issuesOnChart = issues;
         var newItems = issues.GroupBy(SelectedBase.Selector)
                            .Select(group => new EngagementItem(
                               group.Key,
                               SelectedCriteria.Aggregation(group)
                               ));

         DispatcherHelper.CheckBeginInvokeOnUI(() =>
         {
            Items.Clear();
            foreach (var item in newItems)
               Items.Add(item);
         });
      }

      public void Initialize(IMessageBus messageBus)
      {
         messageBus.Register(this);
         //messageBus.Send(new Is)
         //TODO: create RelayCommand inheritor to handle all complexity in determining whether user is connected or not to JIRA
         OpenWindowCommand = new RelayCommand(()=>
         {
            messageBus.Send(new ShowDocumentPaneMessage(this, "Chart - Engagement",
                                                   new EngagementChartControl { DataContext = this },
                                                   new EngagementChartProperties { DataContext = this }));
         }, () => _isLoggedIn);
      }

      public void Handle(LoggedInMessage message)
      {
         _isLoggedIn = true;
         OpenWindowCommand.RaiseCanExecuteChanged();
      }

      public void Handle(LoggedOutMessage message)
      {
         _isLoggedIn = false;
         OpenWindowCommand.RaiseCanExecuteChanged();
      }

      public ObservableCollection<EngagementItem> Items { get; private set; }

      public EngagementCriteria[] AvailableCriterias { get; private set; }
      public EngagementCriteria SelectedCriteria
      {
         get { return _selectedCriteria; }
         set
         {
            if (value == _selectedCriteria)
               return;

            _selectedCriteria = value;
            if (_issuesOnChart != null)
               UpdateData(_issuesOnChart);
         }
      }

      public EngagementBase[] AvailableBases { get; private set; }
      public EngagementBase SelectedBase
      {
         get { return _selectedBase; }
         set
         {
            if (value == _selectedBase)
               return;

            _selectedBase = value;
            if (_issuesOnChart != null)
               UpdateData(_issuesOnChart);
         }
      }

      public class EngagementBase
      {
         public EngagementBase(string name, Func<JiraIssue, string> selector)
         {
            Name = name;
            Selector = selector;
         }

         public string Name { get; private set; }
         public Func<JiraIssue, string> Selector { get; private set; }
      }

      public class EngagementCriteria
      {
         public EngagementCriteria(string name, Func<IEnumerable<JiraIssue>, double> aggregation)
         {
            Name = name;
            Aggregation = aggregation;
         }

         public Func<IEnumerable<JiraIssue>, double> Aggregation { get; private set; }
         public string Name { get; private set; }
      }

      public class EngagementItem
      {
         public EngagementItem(string username, double value)
         {
            Username = username;
            Value = value;
         }

         public string Username { get; private set; }
         public double Value { get; private set; }
      }
   }
}
