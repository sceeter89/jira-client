using JiraAssistant.ViewModel;

namespace JiraAssistant.Pages
{
   public partial class SprintsVelocity
   {
      public SprintsVelocity(SprintsVelocityViewModel viewModel)
      {
         InitializeComponent();

         DataContext = viewModel;
      }

      public override string Title { get { return "Sprints' velocity"; } }
   }
}
