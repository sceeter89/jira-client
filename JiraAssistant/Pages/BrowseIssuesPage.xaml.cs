using System.Windows.Input;
using System;
using System.Windows.Media.Imaging;
using JiraAssistant.Logic.ViewModels;
using JiraAssistant.Domain.Ui;
using JiraAssistant.Controls;

namespace JiraAssistant.Pages
{
    public partial class BrowseIssuesPage
   {
      public BrowseIssuesPage(IssuesBrowserViewModel viewModel)
      {
         InitializeComponent();
         viewModel.Grid = grid;
         Buttons.Add(new ToolbarButton
         {
            Tooltip = "Scrum Cards",
            Command = viewModel.OpenScrumCardsCommand,
            Icon = new BitmapImage(new Uri(@"pack://application:,,,/;component/Assets/Icons/ScrumCard.png"))
         });
         Buttons.Add(new ToolbarButton
         {
            Tooltip = "Export as text",
            Command = viewModel.PlainTextExportCommand,
            Icon = new BitmapImage(new Uri(@"pack://application:,,,/;component/Assets/Icons/TxtFormatIcon.png"))
         });
         Buttons.Add(new ToolbarButton
         {
            Tooltip = "Export as Confluence Markup",
            Command = viewModel.ExportToConfluenceCommand,
            Icon = new BitmapImage(new Uri(@"pack://application:,,,/;component/Assets/Icons/ConfluenceIcon.png"))
         });
         Buttons.Add(new ToolbarButton
         {
            Tooltip = "Save current filter",
            Command = viewModel.SaveFiltersCommand,
            Icon = new BitmapImage(new Uri(@"pack://application:,,,/;component/Assets/Icons/SaveIcon.png"))
         });
         Buttons.Add(new ToolbarButton
         {
            Tooltip = "Load saved filter",
            Command = viewModel.LoadFiltersCommand,
            Icon = new BitmapImage(new Uri(@"pack://application:,,,/;component/Assets/Icons/FilterIcon.png"))
         });

         DataContext = viewModel;
      }

      private void ItemDoubleClick(object sender, MouseButtonEventArgs e)
      {
         (DataContext as IssuesBrowserViewModel).PreviewSelectedIssue();
      }

      public override string Title { get { return "Issues browser"; } }
   }
}
