using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using JiraAssistant.Domain;
using JiraAssistant.Domain.Jira;
using JiraAssistant.Domain.NavigationMessages;
using JiraAssistant.Logic.Services;
using System.Collections.Generic;
using System.Windows.Input;

namespace JiraAssistant.Logic.ViewModels
{
    public class SprintDetailsViewModel : ViewModelBase
    {
        private IssuesCollectionStatistics _statistics;
        private readonly IssuesStatisticsCalculator _statisticsCalculator;
        private readonly IMessenger _messenger;

        public SprintDetailsViewModel(RawAgileSprint sprint,
         IList<JiraIssue> issues,
         IMessenger messenger,
         IssuesStatisticsCalculator statisticsCalculator)
        {
            Sprint = sprint;
            Issues = issues;

            _messenger = messenger;
            _statisticsCalculator = statisticsCalculator;

            ScrumCardsCommand = new RelayCommand(() => _messenger.Send(new OpenScrumCardsMessage(Issues)));
            BurnDownCommand = new RelayCommand(() => _messenger.Send(new OpenBurnDownMessage(Sprint, Issues)));
            EngagementCommand = new RelayCommand(() => _messenger.Send(new OpenEngagementChartMessage(Issues)));
            BrowseIssuesCommand = new RelayCommand(() => _messenger.Send(new OpenIssuesBrowserMessage(Issues)));

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
    }
}
