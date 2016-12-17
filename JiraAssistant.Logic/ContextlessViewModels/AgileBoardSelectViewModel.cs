﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using JiraAssistant.Domain.Exceptions;
using JiraAssistant.Domain.Jira;
using JiraAssistant.Domain.NavigationMessages;
using JiraAssistant.Logic.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using JiraAssistant.Logic.Extensions;
using JiraAssistant.Domain.Messages;

namespace JiraAssistant.Logic.ContextlessViewModels
{
    public class AgileBoardSelectViewModel : ViewModelBase
    {
        private bool _isBusy;
        private string _busyMessage;
        private const int RecentBoardsCount = 3;
        private readonly IJiraApi _jiraApi;
        private readonly AssistantSettings _settings;
        private readonly IMessenger _messenger;

        public AgileBoardSelectViewModel(IMessenger messenger, AssistantSettings settings, IJiraApi jiraApi)
        {
            _messenger = messenger;
            _jiraApi = jiraApi;
            _settings = settings;

            Boards = new ObservableCollection<RawAgileBoard>();
            RecentBoards = new ObservableCollection<RawAgileBoard>();
            OpenBoardCommand = new RelayCommand<RawAgileBoard>(OpenBoard);
            OpenSettingsCommand = new RelayCommand(() => _messenger.Send(new OpenSettingsMessage()));
        }

        private void OpenBoard(RawAgileBoard board)
        {
            UpdateRecentBoardsIdsList(board);
            _messenger.Send(new OpenAgileBoardMessage(board));
        }

        private void UpdateRecentBoardsIdsList(RawAgileBoard board)
        {
            var recentBoardsIds = GetRecentBoardsIds();
            if (recentBoardsIds.Contains(board.Id) == false && recentBoardsIds.Count >= RecentBoardsCount)
            {
                recentBoardsIds.RemoveAt(recentBoardsIds.Count - 1);
            }
            recentBoardsIds.Remove(board.Id);
            recentBoardsIds.Insert(0, board.Id);

            _settings.RecentBoardsIds = string.Join(",", recentBoardsIds);
        }

        private IList<int> GetRecentBoardsIds()
        {
            return _settings.RecentBoardsIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
        }

        public async void OnNavigatedTo()
        {
            Boards.Clear();
            RecentBoards.Clear();

            try
            {
                BusyMessage = "Downloading available agile boards...";
                IsBusy = true;

                var boards = await _jiraApi.Agile.GetAgileBoards();
                var recentBoards = GetRecentBoardsIds();
                foreach (var board in boards.OrderBy(b => b.Name))
                {
                    Boards.Add(board);
                    if (recentBoards.Contains(board.Id))
                        RecentBoards.Add(board);
                }
            }
            catch (MissingJiraAgileSupportException)
            {
                _messenger.Send(new ShowAlertMessage("Please log into JIRA instance with JIRA Agile installed."));
            }
            catch (Exception e)
            {
                Sentry.CaptureException(e);
                _messenger.Send(new ShowAlertMessage("Failed to retrieve list of available JIRA boards. Can't go any further.\nReason: " + e.Message));
            }
            finally
            {
                IsBusy = false;
                BusyMessage = "";
            }
        }

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                RaisePropertyChanged();
            }
        }

        public string BusyMessage
        {
            get { return _busyMessage; }
            set
            {
                _busyMessage = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<RawAgileBoard> Boards { get; private set; }
        public ObservableCollection<RawAgileBoard> RecentBoards { get; private set; }

        public RelayCommand<RawAgileBoard> OpenBoardCommand { get; private set; }
        public RelayCommand OpenSettingsCommand { get; private set; }
    }
}
