using System;
using Yakuza.JiraClient.Api;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.ObjectModel;
using Yakuza.JiraClient.Messages.Actions.Authentication;
using GalaSoft.MvvmLight.Threading;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using System.Windows.Controls;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using Yakuza.JiraClient.Controls.Fields.Search;

namespace Yakuza.JiraClient.Model.SearchableFields
{
   public class SearchByProjectField : ViewModelBase, ISearchableField
   {
      private readonly IJiraOperations _operations;
      private RawProjectInfo _selectedIssueType;

      public SearchByProjectField(IMessenger messenger, IJiraOperations operations)
      {
         _operations = operations;

         ProjectsList = new ObservableCollection<RawProjectInfo>();

         messenger.Register<LoggedInMessage>(this, async m =>
         {
            var projects = await _operations.GetProjectsList();
            if (projects == null)
               return;
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
               ProjectsList.Clear();
               foreach (var project in projects.OrderBy(x => x.Name))
                  ProjectsList.Add(project);
            });
         });
      }

      public ObservableCollection<RawProjectInfo> ProjectsList { get; private set; }

      public bool IsFilled
      {
         get
         {
            return SelectedProject != null;
         }
      }

      public RawProjectInfo SelectedProject
      {
         get { return _selectedIssueType; }
         set
         {
            _selectedIssueType = value;

            RaisePropertyChanged();
         }
      }

      public UserControl EditControl
      {
         get
         {
            return new SearchByProjectFieldControl();
         }
      }

      public ICommand ClearCommand
      {
         get
         {
            return new RelayCommand(() => SelectedProject = null);
         }
      }

      public string GetSearchQuery()
      {
         return string.Format("project = '{0}'", SelectedProject.Name);
      }
   }
}
