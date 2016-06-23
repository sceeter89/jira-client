using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using JiraAssistant.Domain;
using JiraAssistant.Domain.Exceptions;
using JiraAssistant.Domain.Jira;
using JiraAssistant.Domain.NavigationMessages;
using JiraAssistant.Logic.ContextlessViewModels;
using JiraAssistant.Logic.Services;
using JiraAssistant.Logic.Services.Jira;
using NLog;
using System;
using System.Net;
using System.Windows;

namespace JiraAssistant.Logic.ViewModels
{
    public class AgileBoardViewModel : ViewModelBase
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private bool _isBusy;
        private readonly JiraSessionViewModel _jiraSession;
        private readonly IssuesStatisticsCalculator _statisticsCalculator;
        private IssuesCollectionStatistics _statistics;

        private readonly IJiraApi _jiraApi;
        private bool _forceReload;
        private bool _downloadCompleted;
        private AgileBoardIssues _boardContent;
        private readonly IMessenger _messenger;
        private float _downloadProgress;
        private bool _isIndeterminate;

        public AgileBoardViewModel(IJiraApi jiraApi,
           JiraSessionViewModel jiraSession,
           IMessenger messenger,
           IssuesStatisticsCalculator statisticsCalculator,
           RawAgileBoard board)
        {
            Board = board;

            _messenger = messenger;
            _jiraApi = jiraApi;
            _jiraSession = jiraSession;
            _statisticsCalculator = statisticsCalculator;

            PickUpSprintCommand = new RelayCommand(OpenPickUpSprint, () => Board.Type == "scrum");
            OpenPivotAnalysisCommand = new RelayCommand(() => _messenger.Send(new OpenPivotAnalysisMessage(BoardContent.Issues)));
            OpenEpicsOverviewCommand = new RelayCommand(() => _messenger.Send(new OpenEpicsOverviewMessage(BoardContent.Issues, BoardContent.Epics)));
            BrowseIssuesCommand = new RelayCommand(() => _messenger.Send(new OpenIssuesBrowserMessage(BoardContent.Issues)));
            OpenGraveyardCommand = new RelayCommand(() => _messenger.Send(new OpenBoardGraveyardMessage(BoardContent.Issues)));
            OpenSprintsVelocityCommand = new RelayCommand(() => _messenger.Send(new OpenSprintsVelocityMessage(BoardContent)), () => Board.Type == "scrum");

            RefreshDataCommand = new RelayCommand(() =>
            {
                _forceReload = true;
                DownloadElements();
            }, () => IsBusy == false);
            FetchChangesCommand = new RelayCommand(() =>
            {
                DownloadElements();
            }, () => IsBusy == false);

            DownloadElements();
        }

        private void OpenPickUpSprint()
        {
            _messenger.Send(new OpenSprintsPickupMessage(BoardContent));
        }

        public async void DownloadElements()
        {
            IsBusy = true;
            ClearData();

            try
            {
                DownloadProgress = -1;
                BoardContent = await _jiraApi.Agile.GetBoardContent(Board.Id, _forceReload, p => DownloadProgress = p);

                Statistics = await _statisticsCalculator.Calculate(BoardContent.Issues);
                DownloadCompleted = true;
            }
            catch (CacheCorruptedException)
            {
                if (_forceReload)
                {
                    MessageBox.Show("Failed to download board data.", "JiraAssistant");
                    return;
                }
                MessageBox.Show("There were problems to save board data. We'll try to re-download it.", "Jira Assistant");

                _forceReload = true;
                DownloadElements();
            }
            catch (Exception e)
            {
                if (e is WebException && _jiraSession.IsLoggedIn == false)
                {
                    _logger.Trace("Download interrupted due to log out from JIRA.");
                    return;
                }

                _logger.Error(e, "Download failed for issues in board: {0} ({1})", Board.Name, Board.Id);
                ClearData();
                MessageBox.Show("Failed to retrieve issues belonging board: " + Board.Name, "Jira Assistant");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void ClearData()
        {
            BoardContent = null;
            DownloadCompleted = false;
        }

        public bool DownloadCompleted
        {
            get { return _downloadCompleted; }
            set
            {
                _downloadCompleted = value;
                RaisePropertyChanged();
            }
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

        public RawAgileBoard Board { get; private set; }
        public string Title { get { return string.Format("Board: {0}", Board.Name); } }

        public AgileBoardIssues BoardContent
        {
            get { return _boardContent; }
            set
            {
                _boardContent = value;
                RaisePropertyChanged();
            }
        }

        public bool IsBusy
        {
            get { return _isBusy; }
            private set
            {
                _isBusy = value;
                RaisePropertyChanged();
                RefreshDataCommand.RaiseCanExecuteChanged();
                FetchChangesCommand.RaiseCanExecuteChanged();
            }
        }

        public float DownloadProgress
        {
            get { return _downloadProgress; }
            set
            {
                _downloadProgress = value;
                IsIndeterminate = value < 0;
                RaisePropertyChanged();
            }
        }

        public bool IsIndeterminate
        {
            get { return _isIndeterminate; }
            set
            {
                _isIndeterminate = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand PickUpSprintCommand { get; private set; }
        public RelayCommand OpenPivotAnalysisCommand { get; private set; }
        public RelayCommand OpenEpicsOverviewCommand { get; private set; }
        public RelayCommand OpenGraveyardCommand { get; private set; }
        public RelayCommand BrowseIssuesCommand { get; private set; }
        public RelayCommand OpenSprintsVelocityCommand { get; private set; }
        public RelayCommand RefreshDataCommand { get; private set; }
        public RelayCommand FetchChangesCommand { get; private set; }
    }
}
