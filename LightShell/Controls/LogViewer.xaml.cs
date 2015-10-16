using System.Windows;
using Yakuza.JiraClient.ViewModel;

namespace Yakuza.JiraClient.Controls
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
