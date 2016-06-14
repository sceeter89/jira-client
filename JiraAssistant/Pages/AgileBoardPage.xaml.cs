using System;
using System.Windows.Media.Imaging;
using JiraAssistant.Logic.ViewModels;
using JiraAssistant.Domain.Ui;
using JiraAssistant.Logic.Controls;

namespace JiraAssistant.Pages
{
    public partial class AgileBoardPage : BaseNavigationPage
   {
      public AgileBoardPage(AgileBoardViewModel viewModel)
      {
         InitializeComponent();

         Buttons.Add(new ToolbarButton
         {
            Tooltip = "Reload local cache",
            Command = viewModel.RefreshDataCommand,
            Icon = new BitmapImage(new Uri(@"pack://application:,,,/;component/Assets/Icons/RefreshIcon.png"))
         });
         Buttons.Add(new ToolbarButton
         {
            Tooltip = "Fetch changes since last visit",
            Command = viewModel.FetchChangesCommand,
            Icon = new BitmapImage(new Uri(@"pack://application:,,,/;component/Assets/Icons/DownloadIcon.png"))
         });

         StatusBarControl = new AgileBoardPageStatusBar { DataContext = viewModel };

         DataContext = viewModel;
      }

      public override string Title
      {
         get
         {
            return (DataContext as AgileBoardViewModel).Title;
         }
      }
   }
}
