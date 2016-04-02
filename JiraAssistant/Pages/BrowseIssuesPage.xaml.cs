using GalaSoft.MvvmLight.Command;
using JiraAssistant.Model.Jira;
using JiraAssistant.Model.Ui;
using JiraAssistant.Services;
using System.Collections.Generic;
using System.Windows.Input;
using System;
using System.Linq;
using System.Windows.Media.Imaging;
using Autofac;
using Telerik.Windows.Data;

namespace JiraAssistant.Pages
{
   public partial class BrowseIssuesPage
   {
      private readonly INavigator _navigator;
      private readonly IContainer _iocContainer;

      public BrowseIssuesPage(IList<JiraIssue> issues, IContainer iocContainer)
      {
         InitializeComponent();

         Issues = new QueryableCollectionView(issues);
         _iocContainer = iocContainer;
         _navigator = _iocContainer.Resolve<INavigator>();
         ScrumCardsCommand = new RelayCommand(OpenScrumCards);
         ExportCommand = new RelayCommand(ExportResults);
         Buttons.Add(new ToolbarButton
         {
            Tooltip = "Scrum Cards",
            Command = ScrumCardsCommand,
            Icon = new BitmapImage(new Uri(@"pack://application:,,,/;component/Assets/Icons/ScrumCard.png"))
         });
         Buttons.Add(new ToolbarButton
         {
            Tooltip = "Export",
            Command = ExportCommand,
            Icon = new BitmapImage(new Uri(@"pack://application:,,,/;component/Assets/Icons/ExportIcon.png"))
         });

         DataContext = this;
      }

      private void ExportResults()
      {
         throw new NotImplementedException();
      }

      private void OpenScrumCards()
      {
         _navigator.NavigateTo(new ScrumCardsPrintPreview(Issues.OfType<JiraIssue>().ToList(), _iocContainer));
      }

      private void ItemDoubleClick(object sender, MouseButtonEventArgs e)
      {
         if (SelectedIssue != null)
            _navigator.NavigateTo(new IssueDetailsPage(SelectedIssue));
      }

      public QueryableCollectionView Issues { get; private set; }
      public JiraIssue SelectedIssue { get; set; }
      public RelayCommand ScrumCardsCommand { get; private set; }
      public RelayCommand ExportCommand { get; private set; }
   }
}
