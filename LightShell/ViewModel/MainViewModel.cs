using System.Collections.ObjectModel;
using Telerik.Windows.Controls;
using LightShell.Api.Messages.Navigation;
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
      IHandleMessage<ShowDocumentPaneMessage>,
      IHandleMessage<ShowPropertyPaneMessage>,
      IHandleMessage<RemoveDocumentPaneMessage>,
      IHandleMessage<RemovePropertyPaneMessage>,
      IHandleMessage<ClearDocumentPanesMessage>,
      IHandleMessage<ClearPropertyPanesMessage>,
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
         if (_customDocumentPanes.Values.Contains(pane))
         {
            var customPaneKey = _customDocumentPanes.First(x => x.Value == pane).Key;
            _customDocumentPanes.Remove(customPaneKey);
            _customDocumentPaneProperties.Remove(pane);
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
         if (_customDocumentPaneProperties.ContainsKey(documentPane) == false
            || _customDocumentPaneProperties[documentPane] == null)
         {
            CustomPropertyPane.Content = null;
            FocusPropertyPane(SearchPropertyPane);
            return;
         }

         CustomPropertyPane.Content = _customDocumentPaneProperties[documentPane];
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

      public void Handle(UpdateUiBootstrapMessage message)
      {
         DispatcherHelper.CheckBeginInvokeOnUI(() => BusinessMessage = message.Message);
      }

      public void Handle(ApplicationLoadedMessage message)
      {
         DispatcherHelper.CheckBeginInvokeOnUI(() =>
         {
            IsBusy = false;
            BusinessMessage = "";
         });
      }

      public void OnControlInitialized()
      {
         _messageBus.Send(new ViewModelInitializedMessage(this.GetType()));
      }

      private readonly IDictionary<string, RadPane> _customDocumentPanes = new Dictionary<string, RadPane>();
      private readonly IDictionary<string, RadPane> _customPropertyPanes = new Dictionary<string, RadPane>();
      private readonly IDictionary<RadPane, UserControl> _customDocumentPaneProperties = new Dictionary<RadPane, UserControl>();
      public void Handle(ShowDocumentPaneMessage message)
      {
         var paneKey = string.Format("{0}_{1}_{2}", message.Sender.GetType().AssemblyQualifiedName, message.Sender.GetType().FullName, message.Title);
         if (_customDocumentPanes.ContainsKey(paneKey) == false || _customDocumentPanes[paneKey] == null)
         {
            var newPane = new RadPane();
            newPane.CanDockInDocumentHost = true;

            _customDocumentPanes[paneKey] = newPane;
         }

         var pane = _customDocumentPanes[paneKey];
         pane.Content = message.PaneContent;
         pane.Header = message.Title;

         if (message.PaneProperties != null)
         {
            _customDocumentPaneProperties[pane] = message.PaneProperties;
         }

         if (DocumentPanes.Contains(pane) == false)
            DocumentPanes.Add(pane);

         FocusDocumentPane(pane);
      }

      public void Handle(ShowPropertyPaneMessage message)
      {
         var paneKey = string.Format("{0}_{1}_{2}", message.Sender.GetType().AssemblyQualifiedName, message.Sender.GetType().FullName, message.Title);
         if (_customPropertyPanes.ContainsKey(paneKey) == false || _customPropertyPanes[paneKey] == null)
         {
            var newPane = new RadPane();
            newPane.CanDockInDocumentHost = false;
            newPane.CanUserClose = message.IsUserCloseable;

            _customPropertyPanes[paneKey] = newPane;
         }

         var pane = _customPropertyPanes[paneKey];
         pane.Content = message.PaneContent;
         pane.Header = message.Title;

         if (PropertyPanes.Contains(pane) == false)
            PropertyPanes.Add(pane);

         FocusDocumentPane(pane);
      }

      public void Handle(RemoveDocumentPaneMessage message)
      {
         var paneKey = string.Format("{0}_{1}_{2}", message.Sender.GetType().AssemblyQualifiedName, message.Sender.GetType().FullName, message.Title);

         if (_customDocumentPanes.ContainsKey(paneKey) == false)
            return;
         

         var pane = _customDocumentPanes[paneKey];
         _customDocumentPaneProperties.Remove(pane);
         DocumentPanes.Remove(pane);
         _customDocumentPanes.Remove(paneKey);
      }

      public void Handle(RemovePropertyPaneMessage message)
      {
         var paneKey = string.Format("{0}_{1}_{2}", message.Sender.GetType().AssemblyQualifiedName, message.Sender.GetType().FullName, message.Title);

         if(_customPropertyPanes.ContainsKey(paneKey) == false)
            return;

         var pane = _customPropertyPanes[paneKey];
         _customPropertyPanes.Remove(paneKey);
         PropertyPanes.Remove(pane);
      }

      public void Handle(ClearDocumentPanesMessage message)
      {
         throw new NotImplementedException();
      }

      public void Handle(ClearPropertyPanesMessage message)
      {
         throw new NotImplementedException();
      }

      public void PaneCloseAttempt(RadPane pane)
      {
         //TODO: Remove appropriate pane from appropriate section
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