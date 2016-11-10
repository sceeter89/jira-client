using JiraAssistant.Pages;
using JiraAssistant.Logic.ContextlessViewModels;
using System.Windows.Input;

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
                if (viewModel.Settings.StartInTray)
                {
                    viewModel.IsWindowVisible = false;
                }
            };
            Closing += (sender, args) =>
            {
                var viewModel = DataContext as MainViewModel;
                viewModel.HandleClosing(args);
            };
            MouseDown += (sender, args) =>
            {
                if (args.ChangedButton != MouseButton.XButton1)
                    return;

                var viewModel = DataContext as MainViewModel;
                viewModel.Back();
            };
        }
    }
}
