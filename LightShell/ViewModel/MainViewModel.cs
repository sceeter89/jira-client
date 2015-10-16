using LightShell.Controls;
using System.Collections.ObjectModel;
using Telerik.Windows.Controls;
using LightShell.Api.Messages.Navigation;
using LightShell.Api.Messages.Actions.Authentication;
using System.Reflection;
using LightShell.Messaging.Api;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using LightShell.InternalMessages.UI;
using GalaSoft.MvvmLight.Threading;

namespace LightShell.ViewModel
{
   internal class MainViewModel : GalaSoft.MvvmLight.ViewModelBase,
      ICoreViewModel,
      IHandleMessage<LoggedInMessage>,
      IHandleMessage<LoggedOutMessage>,
      IHandleMessage<OpenConnectionTabMessage>,
      IHandleMessage<ShowDocumentPaneMessage>,
      IHandleMessage<UpdateUiBootstrapMessage>,
      IHandleMessage<ApplicationLoadedMessage>
   {
      private readonly IMessageBus _messageBus;
      private int _selectedDocumentPaneIndex;
      private int _selectedPropertyPaneIndex;

      public MainViewModel(IMessageBus messageBus)
      {
         _messageBus = messageBus;

         DocumentPanes = new ObservableCollection<RadPane>();
         PropertyPanes = new ObservableCollection<RadPane> { ConnectionPropertyPane, CustomPropertyPane };
         _messageBus.Register(this);

         Application.Current.DispatcherUnhandledException += DispatcherUnhandledException;

         IsBusy = true;
         BusinessMessage = "Loading application...";
      }

      private void DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
      {
         if (e.Exception.GetType() != typeof(InvalidOperationException))
            return;

         var exception = e.Exception as InvalidOperationException;

         if (exception.Message != "Operation is not valid while ItemsSource is in use. Access and modify elements with ItemsControl.ItemsSource instead."
            || exception.Source != "PresentationFramework")
            return;

         //Most probably user attempted to close active DocumentPane, so let's do it for him
         var pane = DocumentPanes[SelectedDocumentPaneIndex];
         DocumentPanes.Remove(pane);
         if (_customPanes.Values.Contains(pane))
         {
            var customPaneKey = _customPanes.First(x => x.Value == pane).Key;
            _customPanes.Remove(customPaneKey);
            _customPaneProperties.Remove(pane);
         }

         e.Handled = true;
      }

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
            value = Math.Max(0, value);
            _selectedDocumentPaneIndex = value;
            RaisePropertyChanged();
            UpdatePropertiesPane();
         }
      }

      private void UpdatePropertiesPane()
      {
         if (DocumentPanes.Any() == false)
         {
            CustomPropertyPane.Content = null;
            SelectedPropertyPaneIndex = 0;
            return;
         }
         var documentPane = DocumentPanes[SelectedDocumentPaneIndex];
         if (_customPaneProperties.ContainsKey(documentPane) == false
            || _customPaneProperties[documentPane] == null)
         {
            CustomPropertyPane.Content = null;
            FocusPropertyPane(SearchPropertyPane);
            return;
         }

         CustomPropertyPane.Content = _customPaneProperties[documentPane];
         FocusPropertyPane(CustomPropertyPane);
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
         FocusDocumentPane(IssueListDocumentPane);

         PropertyPanes.Clear();
         PropertyPanes.Add(SearchPropertyPane);
         PropertyPanes.Add(ConnectionPropertyPane);
         PropertyPanes.Add(CustomPropertyPane);
         FocusPropertyPane(SearchPropertyPane);
      }

      public void Handle(LoggedOutMessage message)
      {
         DocumentPanes.Clear();
         PropertyPanes.Clear();
         PropertyPanes.Add(ConnectionPropertyPane);
         PropertyPanes.Add(CustomPropertyPane);
      }

      public void Handle(OpenConnectionTabMessage message)
      {
         FocusPropertyPane(ConnectionPropertyPane);
      }

      private readonly IDictionary<string, RadPane> _customPanes = new Dictionary<string, RadPane>();
      private readonly IDictionary<RadPane, UserControl> _customPaneProperties = new Dictionary<RadPane, UserControl>();
      public void Handle(ShowDocumentPaneMessage message)
      {
         var paneKey = string.Format("{0}_{1}", message.Sender.GetType().AssemblyQualifiedName, message.Title);
         if (_customPanes.ContainsKey(paneKey) == false || _customPanes[paneKey] == null)
         {
            var newPane = new RadPane();
            newPane.CanDockInDocumentHost = true;

            _customPanes[paneKey] = newPane;
         }

         var pane = _customPanes[paneKey];
         pane.Content = message.PaneContent;
         pane.Header = message.Title;

         if (message.PaneProperties != null)
         {
            _customPaneProperties[pane] = message.PaneProperties;
         }

         if (DocumentPanes.Contains(pane) == false)
            DocumentPanes.Add(pane);

         FocusDocumentPane(pane);
      }

      public void Handle(UpdateUiBootstrapMessage message)
      {
         DispatcherHelper.CheckBeginInvokeOnUI(() => BusinessMessage = message.Message);
      }

      public void Handle(ApplicationLoadedMessage message)
      {
         DispatcherHelper.CheckBeginInvokeOnUI(()=>
         {
            IsBusy = false;
            BusinessMessage = "";
         });
      }

      public void OnControlInitialized()
      {
         _messageBus.Send(new ViewModelInitializedMessage(this.GetType()));
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

      public bool IsBusy
      {
         get
         {
            return _isBusy;
         }
         set
         {
            _isBusy = value;
            RaisePropertyChanged();
         }
      }

      public string BusinessMessage
      {
         get
         {
            return _businessMessage;
         }
         set
         {
            _businessMessage = value;
            RaisePropertyChanged();
         }
      }

      public ObservableCollection<RadPane> DocumentPanes { get; private set; }
      public ObservableCollection<RadPane> PropertyPanes { get; private set; }

      private RadPane _connectionPropertyPane;
      private RadPane _customPropertyPane;
      private RadPane _searchPropertyPane;

      private RadPane _issueListDocumentPane;
      private bool _isBusy;
      private string _businessMessage;

      private RadPane SearchPropertyPane
      {
         get
         {
            if (_searchPropertyPane == null)
               _searchPropertyPane = new RadPane { Header = "search", Content = new SearchIssues(), CanUserClose = false };

            return _searchPropertyPane;
         }
      }

      private RadPane CustomPropertyPane
      {
         get
         {
            if (_customPropertyPane == null)
               _customPropertyPane = new RadPane { Header = "properties", Content = null, CanUserClose = false };

            return _customPropertyPane;
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
               _issueListDocumentPane = new RadPane { Header = "issues", Content = new IssueListDisplay(), CanUserClose = false };

            return _issueListDocumentPane;
         }
      }
   }
}