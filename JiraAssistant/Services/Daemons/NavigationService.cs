using System;
using Autofac;
using GalaSoft.MvvmLight.Messaging;
using JiraAssistant.Model.NavigationMessages;
using JiraAssistant.Pages;
using JiraAssistant.ViewModel;

namespace JiraAssistant.Services.Daemons
{
   public class NavigationService
   {
      private readonly MainViewModel _mainWindowViewModel;
      private readonly IComponentContext _resolver;

      public NavigationService(IMessenger messenger, MainViewModel mainWindowViewModel, IComponentContext resolver)
      {
         _resolver = resolver;
         _mainWindowViewModel = mainWindowViewModel;
         messenger.Register<OpenAgileBoardMessage>(this, OpenAgileBoard);
         messenger.Register<OpenPivotAnalysisMessage>(this, OpenPivotAnalysis);
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
