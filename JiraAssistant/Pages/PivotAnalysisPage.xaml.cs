using JiraAssistant.ViewModel;

namespace JiraAssistant.Pages
{
   public partial class PivotAnalysisPage
   {

      public PivotAnalysisPage(PivotAnalysisViewModel viewModel)
      {
         InitializeComponent();

         DataContext = viewModel;
      }
      public override string Title { get { return "Pivot analysis"; } }
   }
}
