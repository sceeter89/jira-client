using LightShell.ViewModel;

namespace LightShell.Controls
{
   public partial class ConnectionManager
   {
      public ConnectionManager()
      {
         InitializeComponent();
         Loaded += OnConnectionManagerLoaded;
      }

      private void OnConnectionManagerLoaded(object sender, System.Windows.RoutedEventArgs e)
      {
         (DataContext as ICoreViewModel).OnControlInitialized();
      }
   }
}
