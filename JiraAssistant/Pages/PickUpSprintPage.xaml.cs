using JiraAssistant.Logic.ViewModels;

namespace JiraAssistant.Pages
{
    public partial class PickUpSprintPage : BaseNavigationPage
   {
      public PickUpSprintPage(PickUpSprintViewModel viewModel)
      {
         InitializeComponent();

         DataContext = viewModel;
      }

      public override string Title { get { return "Sprints selection"; } }
   }
}
