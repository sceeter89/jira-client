using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.Windows.Input;
using JiraAssistant.Logic.Services;
using GalaSoft.MvvmLight.Messaging;
using JiraAssistant.Domain.Jira;
using JiraAssistant.Domain;
using JiraAssistant.Domain.NavigationMessages;

namespace JiraAssistant.Pages
{
    public partial class SprintDetailsPage : BaseNavigationPage
   {
      private IssuesCollectionStatistics _statistics;
      private readonly IssuesStatisticsCalculator _statisticsCalculator;
      private readonly IMessenger _messenger;

      public SprintDetailsPage(RawAgileSprint sprint,
         IList<JiraIssue> issues,
         IMessenger messenger,
         IssuesStatisticsCalculator statisticsCalculator)
      {
         InitializeComponent();

         Sprint = sprint;
         Issues = issues;

         _messenger = messenger;
         _statisticsCalculator = statisticsCalculator;

         ScrumCardsCommand = new RelayCommand(() => _messenger.Send(new OpenScrumCardsMessage(Issues)));
         BurnDownCommand = new RelayCommand(() => _messenger.Send(new OpenBurnDownMessage(Sprint, Issues)));
         EngagementCommand = new RelayCommand(() => _messenger.Send(new OpenEngagementChartMessage(Issues)));
         BrowseIssuesCommand = new RelayCommand(() => _messenger.Send(new OpenIssuesBrowserMessage(Issues)));

         DataContext = this;

         GatherStatistics();
      }

      private async void GatherStatistics()
      {
         Statistics = await _statisticsCalculator.Calculate(Issues);
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
      public override string Title { get { return string.Format("Sprint: {0}", Sprint.Name); } }
   }
}
