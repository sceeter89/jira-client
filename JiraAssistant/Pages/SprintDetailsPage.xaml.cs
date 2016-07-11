using JiraAssistant.Logic.ViewModels;

namespace JiraAssistant.Pages
{
    public partial class SprintDetailsPage : BaseNavigationPage
    {
        public SprintDetailsPage(SprintDetailsViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
        }
        public override string Title
        {
            get
            {
                var name = (DataContext as SprintDetailsViewModel).Sprint.Name;
                return string.Format("Sprint: {0}", name);
            }
        }
    }
}
