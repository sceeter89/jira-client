using JiraAssistant.Controls;
using JiraAssistant.Model.Jira;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;

namespace JiraAssistant.Pages
{
   public partial class BurnDownChart
   {
      private Brush _burndownSeriesBrush;
      private DataIndicator _selectedIndicator;
      private readonly RawAgileSprint _sprint;
      private readonly IEnumerable<JiraIssue> _issues;

      public BurnDownChart(RawAgileSprint sprint, IEnumerable<JiraIssue> issues)
      {
         InitializeComponent();

         _sprint = sprint;
         _issues = issues;

         IdealLineSeries = new ObservableCollection<DataPoint>();
         IssuesCountSeries = new ObservableCollection<DataPoint>();
         AvailableIndicators = new ObservableCollection<DataIndicator>
            {
               new DataIndicator { Name = "Issues count", CalculateIssueWeight = i => 1 },
               new DataIndicator { Name = "Story points", CalculateIssueWeight = i => i.StoryPoints }
            };
         SelectedIndicator = AvailableIndicators[0];

         StatusBarControl = new BurnDownChartStatusBar { DataContext = this };

         DataContext = this;
      }

      public Brush BurndownSeriesBrush
      {
         get { return _burndownSeriesBrush; }
         set
         {
            _burndownSeriesBrush = value;
            RaisePropertyChanged();
         }
      }

      public ObservableCollection<DataPoint> IssuesCountSeries { get; private set; }
      public ObservableCollection<DataPoint> IdealLineSeries { get; private set; }

      public ObservableCollection<DataIndicator> AvailableIndicators { get; private set; }
      public DataIndicator SelectedIndicator
      {
         get
         { return _selectedIndicator; }
         set
         {
            _selectedIndicator = value;
            RaisePropertyChanged();
            GenerateChartData();
         }
      }

      private void GenerateChartData()
      {
         IdealLineSeries.Clear();
         IssuesCountSeries.Clear();

         IdealLineSeries.Add(new DataPoint
         {
            Date = _sprint.StartDate.Date,
            Value = _issues.Where(i => i.Created.Date <= _sprint.StartDate && (i.Resolved == null || i.Resolved >= _sprint.StartDate))
                                .Select(SelectedIndicator.CalculateIssueWeight).Sum(),
            ResolvedIssues = _issues.Where(i => i.Resolved.HasValue && i.Resolved.Value.Date == _sprint.StartDate.Date).Count(),
            CreatedIssues = _issues.Where(i => i.Created.Date == _sprint.StartDate.Date).Count()
         });
         IdealLineSeries.Add(new DataPoint
         {
            Date = _sprint.EndDate.Date,
            Value = 0
         });
         var endDate = _sprint.EndDate > DateTime.Now ? DateTime.Today : _sprint.EndDate.Date;
         var iterator = _sprint.StartDate.Date;

         while (iterator <= endDate)
         {
            IssuesCountSeries.Add(new DataPoint
            {
               Date = iterator,
               Value = _issues.Where(i => i.Created.Date <= iterator && (i.Resolved == null || i.Resolved.Value.Date > iterator))
                                   .Select(SelectedIndicator.CalculateIssueWeight).Sum(),
               ResolvedIssues = _issues.Where(i => i.Resolved.HasValue && i.Resolved.Value.Date == iterator).Count(),
               CreatedIssues = _issues.Where(i => i.Created.Date == iterator).Count()
            });
            iterator = iterator.AddDays(1);
         }

         if (_sprint.State != "closed")
            BurndownSeriesBrush = new SolidColorBrush(Color.FromRgb(121, 117, 235));
         else if (IssuesCountSeries.Last().Value > 0)
            BurndownSeriesBrush = new SolidColorBrush(Color.FromRgb(212, 0, 0));
         else
            BurndownSeriesBrush = new SolidColorBrush(Color.FromRgb(0, 181, 27));
      }
   }

   public class DataPoint
   {
      public DateTime Date { get; set; }
      public float Value { get; set; }
      public int CreatedIssues { get; set; }
      public int ResolvedIssues { get; set; }
   }

   public class DataIndicator
   {
      public string Name { get; set; }
      public Func<JiraIssue, float> CalculateIssueWeight { get; set; }
   }
}
