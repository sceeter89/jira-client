using System;
using System.Windows.Media.Imaging;
using JiraAssistant.Logic.Controls;
using JiraAssistant.Logic.ViewModels;
using JiraAssistant.Domain.Ui;

namespace JiraAssistant.Pages
{
    public partial class ScrumCardsPrintPreview : BaseNavigationPage
   {
      public ScrumCardsPrintPreview(ScrumCardsViewModel viewModel)
      {
         InitializeComponent();         

         Buttons.Add(new ToolbarButton
         {
            Tooltip = "Export cards...",
            Command = viewModel.ExportCardsCommand,
            Icon = new BitmapImage(new Uri(@"pack://application:,,,/;component/Assets/Icons/ExportIcon.png"))
         });
         Buttons.Add(new ToolbarControl
         {
            Control = new ScrumCardsPrintPreviewPageIndicator { DataContext = viewModel }
         });
         Buttons.Add(new ToolbarControl
         {
            Control = new ScrumCardsPrintPreviewIssueTypeFilter { DataContext = viewModel }
         });

         DataContext = viewModel;
      }

      public override string Title { get { return "Scrum cards"; } }
   }
}
