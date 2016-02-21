using GalaSoft.MvvmLight.Command;
using JiraAssistant.Model.Jira;
using System.Collections.Generic;
using System.Windows.Input;
using System;
using JiraAssistant.Services;
using JiraAssistant.Model;
using Autofac;

namespace JiraAssistant.Pages
{
   public partial class SprintDetailsPage : BaseNavigationPage
   {
      private readonly INavigator _navigator;
      private IssuesCollectionStatistics _statistics;
      private readonly IssuesStatisticsCalculator _statisticsCalculator;
      private readonly IContainer _iocContainer;

      public SprintDetailsPage(RawAgileSprint sprint,
         IList<JiraIssue> issues,
         IContainer iocContainer)
      {
         InitializeComponent();

         Sprint = sprint;
         Issues = issues;

         _iocContainer = iocContainer;
         _navigator = _iocContainer.Resolve<INavigator>();
         _statisticsCalculator = _iocContainer.Resolve<IssuesStatisticsCalculator>();

         ScrumCardsCommand = new RelayCommand(OpenScrumCards);
         BurnDownCommand = new RelayCommand(OpenBurnDownChart);
         EngagementCommand = new RelayCommand(OpenEngagementChart);
         BrowseIssuesCommand = new RelayCommand(BrowseIssues);

         DataContext = this;

         GatherStatistics();
      }

      private void BrowseIssues()
      {
         _navigator.NavigateTo(new BrowseIssuesPage(Issues, _navigator));
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
         _navigator.NavigateTo(new ScrumCardsPrintPreview(Issues, _iocContainer));
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
      public ICommand BrowseIssuesCommand { get; private set; }
      public RawAgileSprint Sprint { get; private set; }
      public IList<JiraIssue> Issues { get; private set; }
   }
}
