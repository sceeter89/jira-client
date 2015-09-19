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
   public class SearchByResolutionField : ViewModelBase, ISearchableField
   {
      private readonly IJiraOperations _operations;
      private RawResolution _selectedResolution;

      public SearchByResolutionField(IMessenger messenger, IJiraOperations operations)
      {
         _operations = operations;

         ResolutionsList = new ObservableCollection<RawResolution>();

         messenger.Register<LoggedInMessage>(this, async m =>
         {
            var resolutions = await _operations.GetResolutionsList();
            if (resolutions == null)
               return;
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
               ResolutionsList.Clear();
               foreach (var resolution in resolutions.OrderBy(x => x.Name))
                  ResolutionsList.Add(resolution);
            });
         });
      }

      public ObservableCollection<RawResolution> ResolutionsList { get; private set; }

      public bool IsFilled
      {
         get
         {
            return SelectedResolution != null;
         }
      }

      public RawResolution SelectedResolution
      {
         get { return _selectedResolution; }
         set
         {
            _selectedResolution = value;

            RaisePropertyChanged();
         }
      }

      public UserControl EditControl
      {
         get
         {
            return new SearchByResolutionFieldControl();
         }
      }

      public ICommand ClearCommand
      {
         get
         {
            return new RelayCommand(() => SelectedResolution = null);
         }
      }

      public string GetSearchQuery()
      {
         return string.Format("resolution = '{0}'", SelectedResolution.Name);
      }
   }
}
