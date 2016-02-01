using GalaSoft.MvvmLight;
using JiraAssistant.Model.Exceptions;
using JiraAssistant.Model.Jira;
using JiraAssistant.Services.Resources;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace JiraAssistant.ViewModel
{
   public class MainMenuViewModel : ViewModelBase
   {
      private bool _isBusy;
      private string _busyMessage;
      private readonly JiraAgileService _jiraAgile;

      public MainMenuViewModel(JiraAgileService jiraAgile)
      {
         _jiraAgile = jiraAgile;
         Boards = new ObservableCollection<RawAgileBoard>();
      }

      internal async void OnNavigatedTo()
      {
         if (Boards.Any())
            return;

         try
         {
            IsBusy = true;

            var boards = await _jiraAgile.GetAgileBoards();

            foreach (var board in boards)
               Boards.Add(board);
         }
         catch(MissingJiraAgileSupportException) {
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
   }
}
