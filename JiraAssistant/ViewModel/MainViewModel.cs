using System;
using GalaSoft.MvvmLight;
using JiraAssistant.Model.Ui;
using JiraAssistant.Services;
using System.Collections.Generic;
using JiraAssistant.Pages;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Threading;

namespace JiraAssistant.ViewModel
{
   public class MainViewModel : ViewModelBase, INavigator
   {
      private readonly Stack<INavigationPage> _navigationHistory = new Stack<INavigationPage>();
      private INavigationPage _currentPage;
      private AnimationState _collapseAnimationState;
      private AnimationState _expandAnimationState;

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
         await SetPage();
      }

      public async void NavigateTo(INavigationPage page)
      {
         _navigationHistory.Push(page);

         await SetPage();
      }

      public async Task SetPage()
      {
         CollapseAnimationState = AnimationState.Play;
         await Task.Delay(250);
         CollapseAnimationState = AnimationState.Stop;

         await Task.Factory.StartNew(() =>
         {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
               CurrentPage = _navigationHistory.Peek();
            });
         });

         ExpandAnimationState = AnimationState.Play;
         await Task.Delay(250);
         ExpandAnimationState = AnimationState.Stop;
      }
   }
}