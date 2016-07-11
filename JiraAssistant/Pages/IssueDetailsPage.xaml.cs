using JiraAssistant.Logic.ViewModels;

namespace JiraAssistant.Pages
{
    public partial class IssueDetailsPage
    {
        public IssueDetailsPage(IssueDetailsViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
        }

        public override string Title
        {
            get
            {
                var key = (DataContext as IssueDetailsViewModel).Issue.Key;
                return string.Format("Issue: {0}", key);
            }
        }
    }
}