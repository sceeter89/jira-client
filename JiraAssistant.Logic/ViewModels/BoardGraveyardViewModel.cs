using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using JiraAssistant.Domain.Jira;
using JiraAssistant.Domain.NavigationMessages;
using JiraAssistant.Logic.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace JiraAssistant.Logic.ViewModels
{
   public class BoardGraveyardViewModel : ViewModelBase
   {
      private readonly IMessenger _messenger;
      private readonly GraveyardSettings _graveyardSettings;
      private bool _reloadNeeded = true;
      private bool _isBusy;
      private readonly AssistantSettings _appSettings;

      public BoardGraveyardViewModel(IList<JiraIssue> issues,
         IMessenger messenger,
         AssistantSettings appSettings,
         GraveyardSettings graveyardSettings)
      {
         Issues = issues;
         _messenger = messenger;
         _graveyardSettings = graveyardSettings;
         _graveyardSettings.PropertyChanged += (sender, args) => _reloadNeeded = true;
         _appSettings = appSettings;

         OpenDetailsCommand = new RelayCommand<JiraIssue>(issue => _messenger.Send(new OpenIssueDetailsMessage(issue)));
         OpenInBrowserCommand = new RelayCommand<JiraIssue>(issue => Process.Start(string.Format("{0}/browse/{1}", _appSettings.JiraUrl, issue.Key)));

         OldCreated = new ObservableCollection<JiraIssue>();
         ArchaicCreated = new ObservableCollection<JiraIssue>();
         OldUpdated = new ObservableCollection<JiraIssue>();
         InactiveAssignee = new ObservableCollection<JiraIssue>();
         MissingDescription = new ObservableCollection<JiraIssue>();
      }

      public async void RefreshGraveyard()
      {
         if (_reloadNeeded == false)
            return;

         await Task.Factory.StartNew(() =>
         {
            try
            {
               foreach (var issue in Issues.Where(i => i.Resolved.HasValue == false))
               {
                  DispatcherHelper.CheckBeginInvokeOnUI(() =>
                  {
                     var sinceUpdate = DateTime.Now - issue.BuiltInFields.Updated;
                     var sinceCreation = DateTime.Now - issue.BuiltInFields.Created;
                     if (sinceUpdate > _graveyardSettings.UpdateMoreThanBefore)
                        OldUpdated.Add(issue);

                     if (sinceCreation > _graveyardSettings.ArchaicCreatedMoreThanBefore)
                        ArchaicCreated.Add(issue);
                     else if (sinceCreation > _graveyardSettings.CreatedMoreThanBefore)
                        OldCreated.Add(issue);

                     if (issue.BuiltInFields.Assignee != null && issue.BuiltInFields.Assignee.Active == false)
                        InactiveAssignee.Add(issue);

                     if (string.IsNullOrWhiteSpace(issue.Description))
                        MissingDescription.Add(issue);
                  });
               }

               _reloadNeeded = false;
            }
            finally
            {
               IsBusy = false;
            }
         });
      }

      public IList<JiraIssue> Issues { get; private set; }
      public ObservableCollection<JiraIssue> OldUpdated { get; private set; }
      public ObservableCollection<JiraIssue> OldCreated { get; private set; }
      public ObservableCollection<JiraIssue> ArchaicCreated { get; private set; }
      public ObservableCollection<JiraIssue> InactiveAssignee { get; private set; }
      public ObservableCollection<JiraIssue> MissingDescription { get; private set; }

      public RelayCommand<JiraIssue> OpenDetailsCommand { get; private set; }
      public RelayCommand<JiraIssue> OpenInBrowserCommand { get; private set; }

      public bool IsBusy
      {
         get { return _isBusy; }
         set
         {
            _isBusy = value;
            RaisePropertyChanged();
         }
      }
   }
}
