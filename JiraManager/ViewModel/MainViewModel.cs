using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using JiraManager.Api;
using JiraManager.Controls;
using JiraManager.Messages.Actions.Authentication;
using JiraManager.Messages.Navigation;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Xps.Packaging;
using Telerik.Windows.Controls;

namespace JiraManager.ViewModel
{
   public class MainViewModel : GalaSoft.MvvmLight.ViewModelBase
   {
      private bool _isLoggedIn = false;
      private bool _userModifiedQuery = false;

      private readonly SearchIssuesViewModel _searchIssuesViewModel;
      private readonly IMessenger _messenger;
      private int _selectedDocumentPaneIndex;
      private int _selectedPropertyPaneIndex;

      private readonly RadPane _connectionPropertyPane = new RadPane { Header = "JIRA", Content = new ConnectionManager(), CanUserClose = false };
      private readonly RadPane _pivotPropertyPane = new RadPane { Header = "pivot", Content = new PivotReportingProperties() };
      private readonly RadPane _searchPropertyPane = new RadPane { Header = "search", Content = new SearchIssues() };

      private readonly RadPane _issueListDocumentPane = new RadPane { Header = "issues", Content = new IssueListDisplay() };
      private readonly RadPane _pivotDocumentPane = new RadPane { Header = "pivot", Content = new PivotReportingGrid() };

      public MainViewModel(IMessenger messenger, SearchIssuesViewModel searchIssuesViewModel)
      {
         _messenger = messenger;
         _searchIssuesViewModel = searchIssuesViewModel;
         SaveXpsCommand = new RelayCommand(SaveXps, () => _isLoggedIn);

         _messenger.Register<OpenConnectionTabMessage>(this, _ => FocusPropertyPane(_connectionPropertyPane));
         _messenger.Register<LoggedInMessage>(this, LoadUi);
         _messenger.Register<LoggedOutMessage>(this, _ => SetIsLoggedOut());

         DocumentPanes = new ObservableCollection<RadPane>();
         PropertyPanes = new ObservableCollection<RadPane> { _connectionPropertyPane };
      }

      private void LoadUi(LoggedInMessage message)
      {
         DocumentPanes.Clear();
         DocumentPanes.Add(_issueListDocumentPane);
         DocumentPanes.Add(_pivotDocumentPane);
         FocusDocumentPane(_issueListDocumentPane);

         PropertyPanes.Clear();
         PropertyPanes.Add(_searchPropertyPane);
         PropertyPanes.Add(_pivotPropertyPane);
         PropertyPanes.Add(_connectionPropertyPane);
         FocusPropertyPane(_searchPropertyPane);

         SetIsLoggedIn();
      }

      private void SetIsLoggedIn()
      {
         _isLoggedIn = true;
         RefreshCommands();
      }

      private void SetIsLoggedOut()
      {
         _isLoggedIn = false;
         RefreshCommands();
      }

      private void RefreshCommands()
      {
         foreach (var command in GetType().GetProperties().Where(p => p.PropertyType == typeof(RelayCommand)))
         {
            ((RelayCommand)command.GetValue(this)).RaiseCanExecuteChanged();
         }
      }

      private void SaveXps()
      {
         if (_searchIssuesViewModel.FoundIssues.Any() == false)
         {
            _messenger.LogMessage("No issues to export.", LogLevel.Warning);
            return;
         }

         var document = CardsPrintPreview.GenerateDocument(_searchIssuesViewModel.FoundIssues);
         var dlg = new Microsoft.Win32.SaveFileDialog();
         dlg.FileName = "Scrum Cards.xps";
         dlg.DefaultExt = ".xps";
         dlg.Filter = "XPS Documents (.xps)|*.xps";
         dlg.OverwritePrompt = true;

         var result = dlg.ShowDialog();

         if (result == false)
            return;

         var filename = dlg.FileName;
         if (File.Exists(filename))
            File.Delete(filename);

         using (var xpsd = new XpsDocument(filename, FileAccess.ReadWrite))
         {
            var xw = XpsDocument.CreateXpsDocumentWriter(xpsd);
            xw.Write(document);
            xpsd.Close();
         }
      }

      public RelayCommand SaveXpsCommand { get; private set; }

      public int SelectedDocumentPaneIndex
      {
         get
         {
            return _selectedDocumentPaneIndex;
         }
         set
         {
            _selectedDocumentPaneIndex = value;
            RaisePropertyChanged();
            UpdatePropertiesPane();
         }
      }

      private void UpdatePropertiesPane()
      {
         switch (SelectedDocumentPaneIndex)
         {
            case 1: //PivotGrid
               FocusPropertyPane(_pivotPropertyPane);
               break;
         }
      }

      private void FocusPropertyPane(RadPane pane)
      {
         SelectedPropertyPaneIndex = PropertyPanes.IndexOf(pane);
      }

      private void FocusDocumentPane(RadPane pane)
      {
         SelectedDocumentPaneIndex = DocumentPanes.IndexOf(pane);
      }

      public int SelectedPropertyPaneIndex
      {
         get
         {
            return _selectedPropertyPaneIndex;
         }
         set
         {
            _selectedPropertyPaneIndex = value;
            RaisePropertyChanged();
         }
      }

      public ObservableCollection<RadPane> DocumentPanes { get; private set; }
      public ObservableCollection<RadPane> PropertyPanes { get; private set; }
   }
}