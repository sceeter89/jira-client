using System;
using GalaSoft.MvvmLight;
using LightShell.Api;
using LightShell.Messaging.Api;
using LightShell.Plugin.Jira.Api.Messages.IO.Jira;
using System.Windows;
using LightShell.Plugin.Jira.Api.Model;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight.Threading;
using System.Windows.Media;
using System.Windows.Input;
using LightShell.Plugin.Jira.Api;
using LightShell.Api.Messages.Navigation;

namespace LightShell.Plugin.Jira.Agile.Controls
{
   public class BurnDownChartViewModel : ViewModelBase,
      IMicroservice,
      IHandleMessage<SearchForIssuesResponse>,
      IHandleMessage<GetAgileSprintDetailsResponse>
   {
      private readonly Regex _sprintQueryRegex = new Regex(@"^\(\s*sprint = (?<sprintId>\d+)\s*\)$", RegexOptions.IgnoreCase);
      private IMessageBus _messageBus;
      private Visibility _searchForSprintMessageVisibility;
      private RawAgileSprint _selectedSprint;
      private ICollection<JiraIssue> _foundIssues;
      private Brush _burndownSeriesBrush;
      private DataIndicator _selectedIndicator;

      public void Handle(SearchForIssuesResponse message)
      {
         var query = message.Query;
         var match = _sprintQueryRegex.Match(query);
         if (match.Success == false)
         {
            SearchForSprintMessageVisibility = Visibility.Visible;
            return;
         }

         SearchForSprintMessageVisibility = Visibility.Collapsed;
         var sprintId = int.Parse(match.Groups["sprintId"].Value);
         _foundIssues = message.SearchResults;
         _messageBus.Send(new GetAgileSprintDetailsMessage(sprintId));
      }

      public void Initialize(IMessageBus messageBus)
      {
         DispatcherHelper.CheckBeginInvokeOnUI(() =>
         {
            IdealLineSeries = new ObservableCollection<DataPoint>();
            IssuesCountSeries = new ObservableCollection<DataPoint>();
            AvailableIndicators = new ObservableCollection<DataIndicator>
            {
               new DataIndicator { Name = "Issues count", CalculateIssueWeight = i => 1 },
               new DataIndicator { Name = "Story points", CalculateIssueWeight = i => i.StoryPoints }
            };
            SelectedIndicator = AvailableIndicators[0];

            OpenCommand = new LoginEnabledRelayCommand(() =>
            {
               _messageBus.Send(new ShowDocumentPaneMessage(this, "chart - burndown",
                                                            new BurnDownChart { DataContext = this },
                                                            new BurnDownChartProperties { DataContext = this }));
            }, _messageBus);

         });
         _messageBus = messageBus;

         _messageBus.Register(this);
      }

      public void Handle(GetAgileSprintDetailsResponse message)
      {
         SelectedSprint = message.Sprint;
         GenerateChartData(SelectedIndicator, SelectedSprint, _foundIssues);
      }

      private void GenerateChartData(DataIndicator indicator, RawAgileSprint sprint, IEnumerable<JiraIssue> issues)
      {
         IdealLineSeries.Clear();
         IssuesCountSeries.Clear();

         IdealLineSeries.Add(new DataPoint
         {
            Date = sprint.StartDate.Date,
            Value = issues.Where(i => i.Created.Date <= sprint.StartDate && (i.Resolved == null || i.Resolved >= sprint.StartDate))
                                .Select(indicator.CalculateIssueWeight).Sum(),
            ResolvedIssues = issues.Where(i => i.Resolved.HasValue && i.Resolved.Value.Date == sprint.StartDate.Date).Count(),
            CreatedIssues = issues.Where(i => i.Created.Date == sprint.StartDate.Date).Count()
         });
         IdealLineSeries.Add(new DataPoint
         {
            Date = sprint.EndDate.Date,
            Value = 0
         });
         var endDate = sprint.EndDate > DateTime.Now ? DateTime.Today : sprint.EndDate.Date;
         var iterator = sprint.StartDate.Date;

         while (iterator <= endDate)
         {
            IssuesCountSeries.Add(new DataPoint
            {
               Date = iterator,
               Value = issues.Where(i => i.Created.Date <= iterator && (i.Resolved == null || i.Resolved.Value.Date > iterator))
                                   .Select(indicator.CalculateIssueWeight).Sum(),
               ResolvedIssues = issues.Where(i => i.Resolved.HasValue && i.Resolved.Value.Date == iterator).Count(),
               CreatedIssues = issues.Where(i => i.Created.Date == iterator).Count()
            });
            iterator = iterator.AddDays(1);
         }

         if (sprint.State != "closed")
            BurndownSeriesBrush = new SolidColorBrush(Color.FromRgb(121, 117, 235));
         else if (IssuesCountSeries.Last().Value > 0)
            BurndownSeriesBrush = new SolidColorBrush(Color.FromRgb(212, 0, 0));
         else
            BurndownSeriesBrush = new SolidColorBrush(Color.FromRgb(0, 181, 27));
      }

      public Visibility SearchForSprintMessageVisibility
      {
         get { return _searchForSprintMessageVisibility; }
         set
         {
            _searchForSprintMessageVisibility = value;
            RaisePropertyChanged();
         }
      }

      public RawAgileSprint SelectedSprint
      {
         get { return _selectedSprint; }
         set
         {
            _selectedSprint = value;
            RaisePropertyChanged();
         }
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
            if (SelectedSprint != null && _foundIssues != null)
               GenerateChartData(SelectedIndicator, SelectedSprint, _foundIssues);
         }
      }

      public ICommand OpenCommand { get; private set; }

      public class DataPoint
      {
         public DateTime Date { get; set; }
         public int Value { get; set; }
         public int CreatedIssues { get; set; }
         public int ResolvedIssues { get; set; }
      }

      public class DataIndicator
      {
         public string Name { get; set; }
         public Func<JiraIssue, int> CalculateIssueWeight { get; set; }
      }
   }
}
