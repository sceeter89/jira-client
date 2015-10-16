using System.Windows;
using LightShell.ViewModel;

namespace LightShell.Controls
{
   public partial class LogViewer
   {
      public LogViewer()
      {
         InitializeComponent();
         Loaded += OnLogViewerLoaded;
      }

      private void OnLogViewerLoaded(object sender, RoutedEventArgs e)
      {
         (DataContext as ICoreViewModel).OnControlInitialized();
      }
   }
}
