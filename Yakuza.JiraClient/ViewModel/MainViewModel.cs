using GalaSoft.MvvmLight.Command;
using Yakuza.JiraClient.Api;
using Yakuza.JiraClient.Controls;
using System.Collections.ObjectModel;
using System.Linq;
using Telerik.Windows.Controls;
using Yakuza.JiraClient.Api.Messages.Navigation;
using Yakuza.JiraClient.Api.Messages.Actions.Authentication;
using Yakuza.JiraClient.Api.Messages.Actions;
using System.Reflection;
using Yakuza.JiraClient.Messaging.Api;
using Yakuza.JiraClient.Api.Messages.IO.Exports;

namespace Yakuza.JiraClient.ViewModel
{
   public class MainViewModel : GalaSoft.MvvmLight.ViewModelBase,
      IHandleMessage<LoggedInMessage>,
      IHandleMessage<LoggedOutMessage>,
      IHandleMessage<OpenConnectionTabMessage>,
      IHandleMessage<FilteredIssuesListMessage>
   {
      private bool _isLoggedIn = false;

      private readonly IMessageBus _messenger;
      private int _selectedDocumentPaneIndex;
      private int _selectedPropertyPaneIndex;

      public MainViewModel(IMessageBus messenger)
      {
         _messenger = messenger;
         SaveXpsCommand = new RelayCommand(SaveXps, () => _isLoggedIn);
         
         DocumentPanes = new ObservableCollection<RadPane>();
         PropertyPanes = new ObservableCollection<RadPane> { ConnectionPropertyPane };
         _messenger.Register(this);
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
         _messenger.Send(new GetFilteredIssuesListMessage());
      }
      

      public RelayCommand SaveXpsCommand { get; private set; }
      public string AppTitle
      {
         get
         {
            return string.Format("Jira Client - {0}", Assembly.GetExecutingAssembly().GetName().Version.ToString(3));
         }
      }

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

      public void Handle(LoggedInMessage message)
      {
         DocumentPanes.Clear();
         DocumentPanes.Add(IssueListDocumentPane);
         DocumentPanes.Add(PivotDocumentPane);
         FocusDocumentPane(IssueListDocumentPane);

         PropertyPanes.Clear();
         PropertyPanes.Add(SearchPropertyPane);
         PropertyPanes.Add(PivotPropertyPane);
         PropertyPanes.Add(ConnectionPropertyPane);
         FocusPropertyPane(SearchPropertyPane);

         SetIsLoggedIn();
      }

      public void Handle(LoggedOutMessage message)
      {
         SetIsLoggedOut();
      }

      public void Handle(OpenConnectionTabMessage message)
      {
         FocusPropertyPane(ConnectionPropertyPane);
      }

      public void Handle(FilteredIssuesListMessage message)
      {
         if (message.FilteredIssues == null || message.FilteredIssues.Any() == false)
         {
            _messenger.LogMessage("No issues to export.", LogLevel.Warning);
            return;
         }

         _messenger.Send(new GenerateScrumCardsMessage(message.FilteredIssues));
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

      private RadPane _connectionPropertyPane;
      private RadPane _pivotPropertyPane;
      private RadPane _searchPropertyPane;

      private RadPane _issueListDocumentPane;
      private RadPane _pivotDocumentPane;

      private RadPane SearchPropertyPane
      {
         get
         {
            if (_searchPropertyPane == null)
               _searchPropertyPane = new RadPane { Header = "search", Content = new SearchIssues() };

            return _searchPropertyPane;
         }
      }

      private RadPane PivotPropertyPane
      {
         get
         {
            if (_pivotPropertyPane == null)
               _pivotPropertyPane = new RadPane { Header = "pivot", Content = new PivotReportingProperties() };

            return _pivotPropertyPane;
         }
      }

      private RadPane ConnectionPropertyPane
      {
         get
         {
            if (_connectionPropertyPane == null)
               _connectionPropertyPane = new RadPane { Header = "JIRA", Content = new ConnectionManager(), CanUserClose = false };

            return _connectionPropertyPane;
         }
      }

      private RadPane IssueListDocumentPane
      {
         get
         {
            if (_issueListDocumentPane == null)
               _issueListDocumentPane = new RadPane { Header = "issues", Content = new IssueListDisplay() };

            return _issueListDocumentPane;
         }
      }

      private RadPane PivotDocumentPane
      {
         get
         {
            if (_pivotDocumentPane == null)
               _pivotDocumentPane = new RadPane { Header = "pivot", Content = new PivotReportingGrid() };

            return _pivotDocumentPane;
         }
      }
   }
}