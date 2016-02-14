using GalaSoft.MvvmLight.Command;
using JiraAssistant.Model.Jira;
using System.Collections.Generic;
using System.Windows.Input;
using System;
using JiraAssistant.Services;
using JiraAssistant.Model;

namespace JiraAssistant.Pages
{
   public partial class SprintDetailsPage : BaseNavigationPage
   {
      private readonly INavigator _navigator;
      private IssuesCollectionStatistics _statistics;
      private readonly IssuesStatisticsCalculator _statisticsCalculator;

      public SprintDetailsPage(RawAgileSprint sprint,
         IList<JiraIssue> issues,
         INavigator navigator,
         IssuesStatisticsCalculator statisticsCalculator)
      {
         InitializeComponent();

         Sprint = sprint;
         Issues = issues;
         _navigator = navigator;
         _statisticsCalculator = statisticsCalculator;

         ScrumCardsCommand = new RelayCommand(OpenScrumCards);
         BurnDownCommand = new RelayCommand(OpenBurnDownChart);
         EngagementCommand = new RelayCommand(OpenEngagementChart);

         DataContext = this;

         GatherStatistics();
      }

      private async void GatherStatistics()
      {
         Statistics = await _statisticsCalculator.Calculate(Issues);
      }

      private void OpenEngagementChart()
      {
         _navigator.NavigateTo(new EngagementChart(Issues));
      }

      private void OpenBurnDownChart()
      {
         _navigator.NavigateTo(new BurnDownChart(Sprint, Issues));
      }

      private void OpenScrumCards()
      {
         _navigator.NavigateTo(new ScrumCardsPrintPreview(Issues));
      }

      public IssuesCollectionStatistics Statistics
      {
         get { return _statistics; }
         set
         {
            _statistics = value;
            RaisePropertyChanged();
         }
      }

      public ICommand ScrumCardsCommand { get; private set; }
      public ICommand BurnDownCommand { get; private set; }
      public ICommand EngagementCommand { get; private set; }
      public RawAgileSprint Sprint { get; private set; }
      public IList<JiraIssue> Issues { get; private set; }
   }
}
