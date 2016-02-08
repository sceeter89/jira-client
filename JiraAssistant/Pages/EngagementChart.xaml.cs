using GalaSoft.MvvmLight.Threading;
using JiraAssistant.Model.Jira;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace JiraAssistant.Pages
{
   public partial class EngagementChart
   {
      private EngagementCriteria _selectedCriteria;
      private EngagementBase _selectedBase;

      public EngagementChart(IEnumerable<JiraIssue> sprintIssues)
      {
         InitializeComponent();

         Issues = sprintIssues;
         ChartItems = new ObservableCollection<EngagementItem>();

         StatusBarControl = new EngagementChartStatusBar { DataContext = this };
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
         
         DataContext = this;
      }

      private async void UpdateData()
      {
         if (SelectedBase == null || SelectedCriteria == null)
            return;

         var newItems = await Task.Factory.StartNew(() =>
               Issues.GroupBy(SelectedBase.Selector)
                     .Select(group => new EngagementItem(group.Key, SelectedCriteria.Aggregation(group))));

         DispatcherHelper.CheckBeginInvokeOnUI(() =>
         {
            ChartItems.Clear();
            foreach (var item in newItems)
               ChartItems.Add(item);
         });
      }

      public ObservableCollection<EngagementItem> ChartItems { get; private set; }

      public EngagementCriteria[] AvailableCriterias { get; private set; }
      public EngagementCriteria SelectedCriteria
      {
         get { return _selectedCriteria; }
         set
         {
            if (value == _selectedCriteria)
               return;

            _selectedCriteria = value;
            UpdateData();
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
            UpdateData();
         }
      }

      public IEnumerable<JiraIssue> Issues { get; private set; }
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
