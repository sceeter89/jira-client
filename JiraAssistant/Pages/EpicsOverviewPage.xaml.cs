using JiraAssistant.Logic.ViewModels;
using System.Windows.Input;

namespace JiraAssistant.Pages
{
   public partial class EpicsOverviewPage
   {
      public EpicsOverviewPage(EpicsOverviewViewModel viewModel)
      {
         InitializeComponent();
         
         DataContext = viewModel;
      }

      private void ItemDoubleClick(object sender, MouseButtonEventArgs e)
      {
         (DataContext as EpicsOverviewViewModel).OpenSelectedEpic();
      }

      public override string Title { get { return "Epics overview"; } }
   }
}
