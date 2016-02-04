using System;
using GalaSoft.MvvmLight;
using JiraAssistant.Model.Ui;
using JiraAssistant.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Threading;
using GalaSoft.MvvmLight.Command;
using JiraAssistant.Services.Resources;

namespace JiraAssistant.ViewModel
{
   public class MainViewModel : ViewModelBase, INavigator
   {
      private readonly Stack<INavigationPage> _navigationHistory = new Stack<INavigationPage>();
      private INavigationPage _currentPage;
      private AnimationState _collapseAnimationState;
      private AnimationState _expandAnimationState;
      private readonly JiraSessionService _jiraSession;

      public MainViewModel(JiraSessionService jiraSession)
      {
         _jiraSession = jiraSession;
         BackCommand = new RelayCommand(Back, () => _navigationHistory.Count > 1);
      }

      public RelayCommand BackCommand { get; private set; }

      public AnimationState CollapseAnimationState
      {
         get
         {
            return _collapseAnimationState;
         }
         set
         {
            _collapseAnimationState = value;
            RaisePropertyChanged();
         }
      }

      public AnimationState ExpandAnimationState
      {
         get
         {
            return _expandAnimationState;
         }
         set
         {
            _expandAnimationState = value;
            RaisePropertyChanged();
         }
      }

      public INavigationPage CurrentPage
      {
         get
         {
            return _currentPage;
         }
         set
         {
            _currentPage = value;
            RaisePropertyChanged();
         }
      }

      public async void Back()
      {
         if (_navigationHistory.Count == 1)
            return;

         _navigationHistory.Pop();
         if (_navigationHistory.Count == 1)
         {
            _jiraSession.Logout();
         }
         await SetPage();
      }

      public async void NavigateTo(INavigationPage page)
      {
         _navigationHistory.Push(page);

         await SetPage();
      }

      public async Task SetPage()
      {
         await CollapseTab();

         await Task.Factory.StartNew(() =>
         {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
               if (CurrentPage != null)
                  CurrentPage.OnNavigatedFrom();
            });
         });

         await Task.Factory.StartNew(() =>
         {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
               CurrentPage = _navigationHistory.Peek();
            });
         });

         await ExpandTab();

         await Task.Factory.StartNew(() =>
         {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
               CurrentPage.OnNavigatedTo();
               BackCommand.RaiseCanExecuteChanged();
            });
         });
      }

      private async Task ExpandTab()
      {
         ExpandAnimationState = AnimationState.Play;
         await Task.Delay(250);
         ExpandAnimationState = AnimationState.Stop;
      }

      private async Task CollapseTab()
      {
         CollapseAnimationState = AnimationState.Play;
         await Task.Delay(250);
         CollapseAnimationState = AnimationState.Stop;
      }

      public async void ClearHistory()
      {
         while (_navigationHistory.Count > 1)
            _navigationHistory.Pop();

         await SetPage();
      }
   }
}