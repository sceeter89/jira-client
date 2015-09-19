using System.Windows;
using Telerik.Windows.Controls;

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
      }
   }
}
