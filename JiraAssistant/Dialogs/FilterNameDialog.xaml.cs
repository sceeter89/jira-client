using GalaSoft.MvvmLight.Command;
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
         
         var mousePosition = App.Current.MainWindow.PointToScreen(Mouse.GetPosition(null));
         this.Top = mousePosition.Y;
         this.Left = mousePosition.X;
      }

      public RelayCommand AcceptCommand { get; private set; }
      public string FilterName { get; set; }
   }
}
