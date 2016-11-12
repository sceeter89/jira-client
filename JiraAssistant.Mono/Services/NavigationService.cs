using GalaSoft.MvvmLight.Messaging;

namespace JiraAssistant.Mono
{
	public class NavigationService
	{
		private readonly IMessenger _messenger;

		public NavigationService(IMessenger messenger)
		{
			_messenger = messenger;

			/*messenger.Register<OpenAgileBoardMessage>(this, OpenAgileBoard);
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
			messenger.Register<OpenTextualPreviewMessage>(this, OpenTextualPreview);
			messenger.Register<OpenWorklogMessage>(this, OpenWorklog);
			messenger.Register<OpenUpdateAvailableDialogMessage>(this, OpenUpdateAvailable);
			messenger.Register<ShutdownApplicationMessage>(this, ShutdownApplication);*/
		}
	}
}
