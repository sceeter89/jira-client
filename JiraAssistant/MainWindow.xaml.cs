using JiraAssistant.Pages;
using JiraAssistant.Logic.ViewModels;

namespace JiraAssistant
{
   public partial class MainWindow
   {
      public MainWindow()
      {
         InitializeComponent();
         Loaded += (sender, args) =>
         {
            var viewModel = DataContext as MainViewModel;
            viewModel.NavigateTo(new LoginPage());
         };
         Closing += (sender, args) =>
         {
            var viewModel = DataContext as MainViewModel;
            viewModel.HandleClosing(args);
         };
      }
   }
}
