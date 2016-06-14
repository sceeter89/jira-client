using GalaSoft.MvvmLight.Command;
using System.Windows;
using System.Windows.Input;

namespace JiraAssistant.Dialogs
{
   public partial class FilterNameDialog
   {
      public FilterNameDialog()
      {
         InitializeComponent();
         AcceptCommand = new RelayCommand(() => DialogResult = true);
         DataContext = this;
         
         var mousePosition = Application.Current.MainWindow.PointToScreen(Mouse.GetPosition(null));
         this.Top = mousePosition.Y;
         this.Left = mousePosition.X;
      }

      public RelayCommand AcceptCommand { get; private set; }
      public string FilterName { get; set; }
   }
}
