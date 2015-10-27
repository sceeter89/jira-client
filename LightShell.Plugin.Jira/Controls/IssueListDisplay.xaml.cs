namespace LightShell.Plugin.Jira.Controls
{
   public partial class IssueListDisplay
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
