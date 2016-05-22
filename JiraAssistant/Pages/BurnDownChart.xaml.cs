using JiraAssistant.Controls;
using JiraAssistant.ViewModel;

namespace JiraAssistant.Pages
{
   public partial class BurnDownChart
   {

      public BurnDownChart(BurnDownChartViewModel viewModel)
      {
         InitializeComponent();

         StatusBarControl = new BurnDownChartStatusBar { DataContext = viewModel };

         DataContext = viewModel;
      }

      public override string Title { get { return "Burn-down chart"; } }
   }
}
