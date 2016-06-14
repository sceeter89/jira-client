using JiraAssistant.Logic.Controls;
using JiraAssistant.Logic.ViewModels;

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
