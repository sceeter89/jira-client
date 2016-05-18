using Autofac;
using GalaSoft.MvvmLight.Messaging;
using JiraAssistant.Model.NavigationMessages;
using JiraAssistant.Pages;
using JiraAssistant.ViewModel;
using System.Collections.Generic;
using JiraAssistant.Model.Ui;
using System;
using JiraAssistant.Model.Jira;

namespace JiraAssistant.Services.Daemons
{
   public class NavigationService
   {
      private readonly MainViewModel _mainWindowViewModel;
      private readonly IComponentContext _resolver;
      private readonly Dictionary<int, INavigationPage> _sprintsDetailsCache = new Dictionary<int, INavigationPage>();
      private readonly IMessenger _messenger;

      public NavigationService(IMessenger messenger, MainViewModel mainWindowViewModel, IComponentContext resolver)
      {
         _resolver = resolver;
         _mainWindowViewModel = mainWindowViewModel;
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
      }

      private void OpenSprintsPickup(OpenSprintsPickupMessage message)
      {
         var sprints = message.BoardContent.Sprints;
         Func<RawAgileSprint, INavigationPage> followUpCallback = sprint =>
         {
            if (_sprintsDetailsCache.ContainsKey(sprint.Id) == false)
               _sprintsDetailsCache[sprint.Id] = _resolver.Resolve<SprintDetailsPage>(new NamedParameter("sprint", sprint), new NamedParameter("issues", message.BoardContent.IssuesInSprint(sprint.Id)));

            return _sprintsDetailsCache[sprint.Id];
         };
         var page = _resolver.Resolve<PickUpSprintPage>(new NamedParameter("sprints", sprints), new NamedParameter("followUp", followUpCallback)); 
         
         _mainWindowViewModel.NavigateTo(page);
      }

      private void OpenPage(OpenPageMessage message)
      {
         _mainWindowViewModel.NavigateTo(message.Page);
      }

      private void OpenSprintsVelocity(OpenSprintsVelocityMessage message)
      {
         var viewModel = _resolver.Resolve<SprintsVelocityViewModel>(new NamedParameter("issues", message.Issues));
         var page = new SprintsVelocity(viewModel);

         _mainWindowViewModel.NavigateTo(page);
      }

      private void OpenBoardGraveyard(OpenBoardGraveyardMessage message)
      {
         var viewModel = _resolver.Resolve<BoardGraveyardViewModel>(new NamedParameter("issues", message.Issues));
         var page = new BoardGraveyard(viewModel);

         _mainWindowViewModel.NavigateTo(page);
      }

      private void OpenIssueDetails(OpenIssueDetailsMessage message)
      {
         var page = _resolver.Resolve<IssueDetailsPage>(new NamedParameter("issue", message.Issue));

         _mainWindowViewModel.NavigateTo(page);
      }

      private void OpenScrumCards(OpenScrumCardsMessage message)
      {
         var viewModel = _resolver.Resolve<ScrumCardsViewModel>(new NamedParameter("issues", message.Issues));
         var page = new ScrumCardsPrintPreview(viewModel);

         _mainWindowViewModel.NavigateTo(page);
      }

      private void OpenIssuesBrowser(OpenIssuesBrowserMessage message)
      {
         var viewModel = _resolver.Resolve<IssuesBrowserViewModel>(new NamedParameter("issues", message.Issues));
         var page = new BrowseIssuesPage(viewModel);

         _mainWindowViewModel.NavigateTo(page);
      }

      private void OpenEpicsOverview(OpenEpicsOverviewMessage message)
      {
         var viewModel = _resolver.Resolve<EpicsOverviewViewModel>(
            new NamedParameter("issues", message.Issues),
            new NamedParameter("epics", message.Epics)
            );
         var page = new EpicsOverviewPage(viewModel);

         _mainWindowViewModel.NavigateTo(page);
      }

      private void OpenPivotAnalysis(OpenPivotAnalysisMessage message)
      {
         var viewModel = _resolver.Resolve<PivotAnalysisViewModel>(new NamedParameter("issues", message.Issues));
         var page = new PivotAnalysisPage(viewModel);

         _mainWindowViewModel.NavigateTo(page);
      }

      private void OpenAgileBoard(OpenAgileBoardMessage message)
      {
         var viewModel = _resolver.Resolve<AgileBoardViewModel>(new NamedParameter("board", message.Board));
         var page = new AgileBoardPage(viewModel);

         _mainWindowViewModel.NavigateTo(page);
      }
   }
}
