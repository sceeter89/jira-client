using System.Windows;
using Telerik.Windows.Controls;
using LightShell.ViewModel;
using System.Linq;

namespace LightShell
{
   public partial class MainWindow
   {
      public MainWindow()
      {
         StyleManager.ApplicationTheme = new Windows8Theme();
         InitializeComponent();
         Loaded += OnMainWindowLoaded;
      }

      private void OnMainWindowLoaded(object sender, RoutedEventArgs e)
      {
         (DataContext as ICoreViewModel).OnControlInitialized();
      }

      private void PaneClosePreview(object sender, Telerik.Windows.Controls.Docking.StateChangeEventArgs e)
      {
         if (e == null || e.Panes == null || e.Panes.Any() == false || e.Panes.Count() > 0)
            return;

         (DataContext as MainViewModel).PaneCloseAttempt(e.Panes.First());
      }
   }
}
