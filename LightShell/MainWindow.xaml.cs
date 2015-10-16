using System.Windows;
using Telerik.Windows.Controls;
using Yakuza.JiraClient.ViewModel;

namespace Yakuza.JiraClient
{
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow : Window
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
   }
}
