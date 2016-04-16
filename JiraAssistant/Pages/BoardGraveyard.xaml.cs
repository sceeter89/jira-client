using Autofac;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using JiraAssistant.Model.Jira;
using JiraAssistant.Services;
using JiraAssistant.Services.Settings;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace JiraAssistant.Pages
{
   public partial class BoardGraveyard
   {
      private readonly INavigator _navigator;
      private readonly GraveyardSettings _settings;
      private bool _reloadNeeded = true;
      private bool _isBusy;
      private readonly AssistantSettings _appSettings;

      public BoardGraveyard(IList<JiraIssue> issues, IContainer iocContainer)
      {
         InitializeComponent();

         Issues = issues;
         _navigator = iocContainer.Resolve<INavigator>();
         _settings = iocContainer.Resolve<GraveyardSettings>();
         _settings.PropertyChanged += (sender, args) => _reloadNeeded = true;
         _appSettings = iocContainer.Resolve<AssistantSettings>();
         
         OpenDetailsCommand = new RelayCommand<JiraIssue>(issue => _navigator.NavigateTo(new IssueDetailsPage(issue)));
         OpenInBrowserCommand = new RelayCommand<JiraIssue>(issue => Process.Start(string.Format("{0}/browse/{1}", _appSettings.JiraUrl, issue.Key)));

         OldCreated = new ObservableCollection<JiraIssue>();
         ArchaicCreated = new ObservableCollection<JiraIssue>();
         OldUpdated = new ObservableCollection<JiraIssue>();
         InactiveAssignee = new ObservableCollection<JiraIssue>();
         MissingDescription = new ObservableCollection<JiraIssue>();

         DataContext = this;
      }

      public override void OnNavigatedTo()
      {
         if (_reloadNeeded == false)
            return;

         Task.Factory.StartNew(() =>
         {
            try
            {
               foreach (var issue in Issues.Where(i => i.Resolved.HasValue == false))
               {
                  DispatcherHelper.CheckBeginInvokeOnUI(() =>
                  {
                     var sinceUpdate = DateTime.Now - issue.BuiltInFields.Updated;
                     var sinceCreation = DateTime.Now - issue.BuiltInFields.Created;
                     if (sinceUpdate > _settings.UpdateMoreThanBefore)
                        OldUpdated.Add(issue);

                     if (sinceCreation > _settings.ArchaicCreatedMoreThanBefore)
                        ArchaicCreated.Add(issue);
                     else if (sinceCreation > _settings.CreatedMoreThanBefore)
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
