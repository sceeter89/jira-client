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
      private readonly Regex _sprintQueryRegex = new Regex(@"\(\s*sprint = (?<sprintId>\d+)\s*\)", RegexOptions.IgnoreCase);
      private IMessageBus _messageBus;
      private Visibility _searchForSprintMessageVisibility;
      private RawAgileSprint _selectedSprint;
      private ICollection<JiraIssue> _foundIssues;
      private Brush _burndownSeriesBrush;
      private List<ObservableCollection<DataPoint>> _dataSeries;

      public void Handle(SearchForIssuesResponse message)
      {
         IdealLineSeries.Clear();
         IssuesCountSeries.Clear();

         var query = message.Query;
         var match = _sprintQueryRegex.Match(query);
         if (match.Success == false)
         {
            ClearAndWaitForNewResults();
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

            OpenCommand = new LoginEnabledRelayCommand(() =>
            {
               _messageBus.Send(new ShowDocumentPaneMessage(this, "chart - burndown",
                                                            new BurnDownChart { DataContext = this }));
            }, _messageBus);
         });
         _messageBus = messageBus;

         _messageBus.Register(this);
      }

      public void Handle(GetAgileSprintDetailsResponse message)
      {
         SelectedSprint = message.Sprint;
         IdealLineSeries.Add(new DataPoint
         {
            Date = SelectedSprint.StartDate.Date,
            Value = _foundIssues.Count
         });
         IdealLineSeries.Add(new DataPoint
         {
            Date = SelectedSprint.EndDate.Date,
            Value = 0
         });
         var endDate = SelectedSprint.EndDate > DateTime.Now ? DateTime.Today : SelectedSprint.EndDate.Date;
         var iterator = SelectedSprint.StartDate.Date;

         while (iterator <= endDate)
         {
            IssuesCountSeries.Add(new DataPoint
            {
               Date = iterator,
               Value = _foundIssues.Where(i => i.Resolved == null || i.Resolved.Value > iterator).Count()
            });
            iterator = iterator.AddDays(1);
         }

         if (SelectedSprint.State != "closed")
            BurndownSeriesBrush = new SolidColorBrush(Colors.CadetBlue);
         else if (IssuesCountSeries.Last().Value > 0)
            BurndownSeriesBrush = new SolidColorBrush(Colors.Coral);
         else
            BurndownSeriesBrush = new SolidColorBrush(Colors.ForestGreen);
      }

      private void ClearAndWaitForNewResults()
      {
         SearchForSprintMessageVisibility = Visibility.Collapsed;
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

      public ICommand OpenCommand { get; private set; }

      public class DataPoint
      {
         public DateTime Date { get; set; }
         public int Value { get; set; }
      }
   }
}
