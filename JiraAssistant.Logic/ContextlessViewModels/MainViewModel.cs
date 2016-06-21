using GalaSoft.MvvmLight;
using JiraAssistant.Logic.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Threading;
using GalaSoft.MvvmLight.Command;
using JiraAssistant.Logic.Services.Jira;
using System.Collections.ObjectModel;
using System.Windows;
using System.ComponentModel;
using JiraAssistant.Logic.Services.Daemons;
using JiraAssistant.Domain.Ui;
using JiraAssistant.Logic.Settings;
using JiraAssistant.Domain.Exceptions;
using GalaSoft.MvvmLight.Messaging;
using JiraAssistant.Domain.NavigationMessages;
using System.Reflection;

namespace JiraAssistant.Logic.ContextlessViewModels
{
    public class MainViewModel : ViewModelBase, INavigator
    {
        private readonly Stack<INavigationPage> _navigationHistory = new Stack<INavigationPage>();
        private INavigationPage _currentPage;
        private AnimationState _collapseAnimationState;
        private AnimationState _expandAnimationState;
        private readonly IJiraApi _jiraApi;
        private string _userMessage;
        private Visibility _windowVisibility;
        private readonly IMessenger _messenger;

        public MainViewModel(IJiraApi jiraApi, GeneralSettings settings, WorkLogUpdater workLogUpdater, IMessenger messenger)
        {
            _jiraApi = jiraApi;
            _messenger = messenger;
            BackCommand = new RelayCommand(Back, () => _navigationHistory.Count > 1);
            ClearMessageCommand = new RelayCommand(() => { UserMessage = ""; });
            this.OpenSettingsCommand = new RelayCommand(OpenSettings, () => this._navigationHistory.Count > 1 && this._navigationHistory.Peek().GetType().Name != "ApplicationSettings");
            LogWorkCommand = workLogUpdater.LogWorkCommand;
            BackToPageCommand = new RelayCommand<NavigationHistoryEntry>(BackToPage);
            CloseApplicationCommand = new RelayCommand(CloseApplication);
            NavigationHistory = new ObservableCollection<NavigationHistoryEntry>();
            Settings = settings;

            ActivateWindowCommand = new RelayCommand(() => WindowVisibility = Visibility.Visible);
        }

        private void OpenSettings()
        {
            _messenger.Send(new OpenSettingsMessage());
        }

        private void CloseApplication()
        {
            Application.Current.Shutdown();
        }

        public RelayCommand BackCommand { get; private set; }
        public RelayCommand ClearMessageCommand { get; private set; }

        public AnimationState CollapseAnimationState
        {
            get { return _collapseAnimationState; }
            set
            {
                _collapseAnimationState = value;
                RaisePropertyChanged();
            }
        }

        public AnimationState ExpandAnimationState
        {
            get { return _expandAnimationState; }
            set
            {
                _expandAnimationState = value;
                RaisePropertyChanged();
            }
        }

        public INavigationPage CurrentPage
        {
            get { return _currentPage; }
            set
            {
                _currentPage = value;
                RaisePropertyChanged();
            }
        }

        public string ApplicationTitle
        {
            get { return string.Format("Jira Assistant - {0}", Assembly.GetEntryAssembly().GetName().Version.ToString(3)); }
        }

        public string UserMessage
        {
            get { return _userMessage; }
            set
            {
                _userMessage = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand OpenSettingsCommand { get; private set; }
        public ObservableCollection<NavigationHistoryEntry> NavigationHistory { get; private set; }
        public RelayCommand<NavigationHistoryEntry> BackToPageCommand { get; private set; }
        public GeneralSettings Settings { get; private set; }
        public Visibility WindowVisibility
        {
            get { return _windowVisibility; }
            set
            {
                _windowVisibility = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand ActivateWindowCommand { get; private set; }
        public RelayCommand LogWorkCommand { get; private set; }
        public RelayCommand CloseApplicationCommand { get; private set; }

        private async void BackToPage(NavigationHistoryEntry entry)
        {
            if (_navigationHistory.Peek() == entry.Page)
                return;

            while (_navigationHistory.Peek() != entry.Page)
            {
                _navigationHistory.Pop();
                NavigationHistory.RemoveAt(0);
            }

            await SetPage();
        }

        public async void Back()
        {
            if (_navigationHistory.Count == 1)
                return;

            _navigationHistory.Pop();
            NavigationHistory.RemoveAt(0);
            if (_navigationHistory.Count == 1)
            {
                try
                {
                    await _jiraApi.Session.Logout();
                }
                catch (IncompleteJiraConfiguration)
                { }
            }
            await SetPage();
        }

        public async void NavigateTo(INavigationPage page)
        {
            _navigationHistory.Push(page);
            NavigationHistory.Insert(0, new NavigationHistoryEntry(page));
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
                   OpenSettingsCommand.RaiseCanExecuteChanged();
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
            {
                _navigationHistory.Pop();
                NavigationHistory.RemoveAt(0);
            }

            await SetPage();
        }

        public void HandleClosing(CancelEventArgs args)
        {
            if (Settings.CloseToTray)
            {
                WindowVisibility = Visibility.Hidden;
                args.Cancel = true;
            }
        }
    }

    public class NavigationHistoryEntry
    {
        public NavigationHistoryEntry(INavigationPage page)
        {
            Page = page;
            Title = page.Title;
        }
        public INavigationPage Page { get; private set; }
        public string Title { get; private set; }
    }
}