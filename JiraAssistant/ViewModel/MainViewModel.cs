using GalaSoft.MvvmLight;
using JiraAssistant.Model.Ui;
using JiraAssistant.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Threading;
using GalaSoft.MvvmLight.Command;
using JiraAssistant.Services.Jira;

namespace JiraAssistant.ViewModel
{
   public class MainViewModel : ViewModelBase, INavigator
   {
      private readonly Stack<INavigationPage> _navigationHistory = new Stack<INavigationPage>();
      private INavigationPage _currentPage;
      private AnimationState _collapseAnimationState;
      private AnimationState _expandAnimationState;
      private readonly IJiraApi _jiraApi;

      public MainViewModel(IJiraApi jiraApi)
      {
         _jiraApi = jiraApi;
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

      public string ApplicationTitle
      {
         get { return string.Format("JIRA Assistant - {0}", GetType().Assembly.GetName().Version.ToString(3)); }
      }

      public async void Back()
      {
         if (_navigationHistory.Count == 1)
            return;

         _navigationHistory.Pop();
         if (_navigationHistory.Count == 1)
         {
            await _jiraApi.Session.Logout();
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
         CollapseTab();

         await Task.Factory.StartNew(() =>
         {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
               if (CurrentPage != null)
                  CurrentPage.OnNavigatedFrom();
            });
         });

         await Task.Delay(250);
         await Task.Factory.StartNew(() =>
         {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
               CurrentPage = _navigationHistory.Peek();
            });
         });

         await Task.Delay(200);
         ExpandTab();

         await Task.Factory.StartNew(() =>
         {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
               CurrentPage.OnNavigatedTo();
               BackCommand.RaiseCanExecuteChanged();
            });
         });
      }

      private void ExpandTab()
      {
         ExpandAnimationState = AnimationState.Stop;
         ExpandAnimationState = AnimationState.Play;
      }

      private void CollapseTab()
      {
         CollapseAnimationState = AnimationState.Stop;
         CollapseAnimationState = AnimationState.Play;
      }

      public async void ClearHistory()
      {
         while (_navigationHistory.Count > 1)
            _navigationHistory.Pop();

         await SetPage();
      }
   }
}