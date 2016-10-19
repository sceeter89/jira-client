using JiraAssistant.Controls;
using JiraAssistant.Logic.ViewModels;

namespace JiraAssistant.Pages
{
    public partial class SprintsVelocity
    {
        public SprintsVelocity(SprintsVelocityViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
            StatusBarControl = new SprintsVelocityLegend { DataContext = viewModel };
        }

        public override string Title { get { return "Sprints' velocity"; } }
    }
}
