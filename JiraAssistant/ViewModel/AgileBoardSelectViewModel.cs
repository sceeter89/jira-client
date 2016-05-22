using Autofac;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using JiraAssistant.Model.Exceptions;
using JiraAssistant.Model.Jira;
using JiraAssistant.Model.NavigationMessages;
using JiraAssistant.Model.Ui;
using JiraAssistant.Pages;
using JiraAssistant.Services;
using JiraAssistant.Services.Jira;
using JiraAssistant.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace JiraAssistant.ViewModel
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

      internal async void OnNavigatedTo()
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
            MessageBox.Show("Please log into JIRA instance with JIRA Agile installed.", "JIRA Assistant", MessageBoxButton.OK, MessageBoxImage.Information);
         }
         catch (Exception e)
         {
            MessageBox.Show("Failed to retrieve list of available JIRA boards. Can't go any further.\nReason: " + e.Message, "JIRA Assistant", MessageBoxButton.OK, MessageBoxImage.Warning);
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
