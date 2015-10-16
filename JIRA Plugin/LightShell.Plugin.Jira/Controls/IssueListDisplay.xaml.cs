using System.Windows.Controls;
using LightShell.ViewModel;

namespace LightShell.Controls
{
   public partial class IssueListDisplay : UserControl
   {
      public IssueListDisplay()
      {
         InitializeComponent();
      }

      private void ItemDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
      {
         var viewModel = DataContext as IssueListViewModel;
         viewModel.RowDoubleClickedCommand.Execute(null);
      }
   }
}
