using Autofac;
using GalaSoft.MvvmLight.Messaging;
using JiraAssistant.Pages;
using System.Collections.Generic;
using System;
using JiraAssistant.Domain.NavigationMessages;
using JiraAssistant.Logic.ViewModels;
using JiraAssistant.Domain.Ui;
using JiraAssistant.Domain.Jira;
using JiraAssistant.Logic.Services;

namespace JiraAssistant
{
    public class NavigationService
    {
        private readonly INavigator _navigator;
        private readonly IComponentContext _resolver;
        private readonly IDictionary<int, INavigationPage> _sprintsDetailsCache = new Dictionary<int, INavigationPage>();
        private readonly IDictionary<int, INavigationPage> _boardPagesCache = new Dictionary<int, INavigationPage>();
        private readonly IMessenger _messenger;

        public NavigationService(IMessenger messenger, INavigator navigator, IComponentContext resolver)
        {
            _resolver = resolver;
            _navigator = navigator;
            _messenger = messenger;

            messenger.Register<OpenAgileBoardMessage>(this, OpenAgileBoard);
            messenger.Register<OpenPivotAnalysisMessage>(this, OpenPivotAnalysis);
            messenger.Register<OpenEpicsOverviewMessage>(this, OpenEpicsOverview);
            messenger.Register<OpenIssuesBrowserMessage>(this, OpenIssuesBrowser);
            messenger.Register<OpenScrumCardsMessage>(this, OpenScrumCards);
            messenger.Register<OpenIssueDetailsMessage>(this, OpenIssueDetails);
            messenger.Register<OpenBoardGraveyardMessage>(this, OpenBoardGraveyard);
            messenger.Register<OpenSprintsVelocityMessage>(this, OpenSprintsVelocity);
            messenger.Register<OpenPageMessage>(this, OpenPage);
            messenger.Register<OpenSprintsPickupMessage>(this, OpenSprintsPickup);
            messenger.Register<OpenBurnDownMessage>(this, OpenBurnDown);
            messenger.Register<OpenEngagementChartMessage>(this, OpenEngagementChart);
            messenger.Register<OpenSettingsMessage>(this, OpenSettings);
            messenger.Register<OpenAgileBoardPickupMessage>(this, OpenAgileBoardPickup);
            messenger.Register<ClearNavigationHistoryMessage>(this, ClearNavigationStack);
            messenger.Register<OpenRecentUpdatesMessage>(this, OpenRecentUpdates);
        }

        private void OpenRecentUpdates(OpenRecentUpdatesMessage message)
        {
            var page = _resolver.Resolve<RecentUpdates>();

            _navigator.NavigateTo(page);
        }

        private void OpenAgileBoardPickup(OpenAgileBoardPickupMessage message)
        {
            var page = _resolver.Resolve<PickUpAgileBoardPage>();
                                             
            _navigator.NavigateTo(page);
        }

        private void ClearNavigationStack(ClearNavigationHistoryMessage message)
        {
            _navigator.ClearHistory();
        }

        private void OpenSettings(OpenSettingsMessage message)
        {
            var page = _resolver.Resolve<ApplicationSettings>(new NamedParameter("initialPage", message.InitialPage));

            _navigator.NavigateTo(page);
        }

        private void OpenEngagementChart(OpenEngagementChartMessage message)
        {
            var viewModel = _resolver.Resolve<EngagementChartViewModel>(new NamedParameter("sprintIssues", message.Issues));
            var page = new EngagementChart(viewModel);

            _navigator.NavigateTo(page);
        }

        private void OpenBurnDown(OpenBurnDownMessage message)
        {
            var viewModel = _resolver.Resolve<BurnDownChartViewModel>(new NamedParameter("issues", message.Issues), new NamedParameter("sprint", message.Sprint));
            var page = new BurnDownChart(viewModel);

            _navigator.NavigateTo(page);
        }

        private void OpenSprintsPickup(OpenSprintsPickupMessage message)
        {
            var sprints = message.BoardContent.Sprints;
            Func<RawAgileSprint, INavigationPage> followUpCallback = sprint =>
            {
                if (_sprintsDetailsCache.ContainsKey(sprint.Id) == false)
                {
                    var newViewModel = _resolver.Resolve<SprintDetailsViewModel>(new NamedParameter("sprint", sprint), new NamedParameter("issues", message.BoardContent.IssuesInSprint(sprint.Id)));
                    _sprintsDetailsCache[sprint.Id] = new SprintDetailsPage(newViewModel);
                }

                return _sprintsDetailsCache[sprint.Id];
            };
            var viewModel = _resolver.Resolve<PickUpSprintViewModel>(new NamedParameter("sprints", sprints), new NamedParameter("followUp", followUpCallback));
            var page = new PickUpSprintPage(viewModel);

            _navigator.NavigateTo(page);
        }

        private void OpenPage(OpenPageMessage message)
        {
            _navigator.NavigateTo(message.Page);
        }

        private void OpenSprintsVelocity(OpenSprintsVelocityMessage message)
        {
            var viewModel = _resolver.Resolve<SprintsVelocityViewModel>(new NamedParameter("issues", message.Issues));
            var page = new SprintsVelocity(viewModel);

            _navigator.NavigateTo(page);
        }

        private void OpenBoardGraveyard(OpenBoardGraveyardMessage message)
        {
            var viewModel = _resolver.Resolve<BoardGraveyardViewModel>(new NamedParameter("issues", message.Issues));
            var page = new BoardGraveyard(viewModel);

            _navigator.NavigateTo(page);
        }

        private void OpenIssueDetails(OpenIssueDetailsMessage message)
        {
            var viewModel = _resolver.Resolve<IssueDetailsViewModel>(new NamedParameter("issue", message.Issue));
            var page = new IssueDetailsPage(viewModel);

            _navigator.NavigateTo(page);
        }

        private void OpenScrumCards(OpenScrumCardsMessage message)
        {
            var viewModel = _resolver.Resolve<ScrumCardsViewModel>(new NamedParameter("issues", message.Issues));
            var page = new ScrumCardsPrintPreview(viewModel);

            _navigator.NavigateTo(page);
        }

        private void OpenIssuesBrowser(OpenIssuesBrowserMessage message)
        {
            var viewModel = _resolver.Resolve<IssuesBrowserViewModel>(new NamedParameter("issues", message.Issues));
            var page = new BrowseIssuesPage(viewModel);

            _navigator.NavigateTo(page);
        }

        private void OpenEpicsOverview(OpenEpicsOverviewMessage message)
        {
            var viewModel = _resolver.Resolve<EpicsOverviewViewModel>(
               new NamedParameter("issues", message.Issues),
               new NamedParameter("epics", message.Epics)
               );
            var page = new EpicsOverviewPage(viewModel);

            _navigator.NavigateTo(page);
        }

        private void OpenPivotAnalysis(OpenPivotAnalysisMessage message)
        {
            var viewModel = _resolver.Resolve<PivotAnalysisViewModel>(new NamedParameter("issues", message.Issues));
            var page = new PivotAnalysisPage(viewModel);

            _navigator.NavigateTo(page);
        }

        private void OpenAgileBoard(OpenAgileBoardMessage message)
        {
            if (_boardPagesCache.ContainsKey(message.Board.Id))
            {
                _navigator.NavigateTo(_boardPagesCache[message.Board.Id]);
                return;
            }

            var viewModel = _resolver.Resolve<AgileBoardViewModel>(new NamedParameter("board", message.Board));
            var page = new AgileBoardPage(viewModel);
            _boardPagesCache[message.Board.Id] = page;
            _navigator.NavigateTo(page);
        }
    }
}
