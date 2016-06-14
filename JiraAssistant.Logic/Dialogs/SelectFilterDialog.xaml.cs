using GalaSoft.MvvmLight.Command;
using System.Windows;
using System.Windows.Input;

namespace JiraAssistant.Dialogs
{
   public partial class SelectFilterDialog
   {
      public SelectFilterDialog(string[] filters)
      {
         InitializeComponent();
         AcceptCommand = new RelayCommand(() => DialogResult = true);
         Options = filters;
         DataContext = this;

         var mousePosition = Application.Current.MainWindow.PointToScreen(Mouse.GetPosition(null));
         this.Top = mousePosition.Y;
         this.Left = mousePosition.X;
      }

      public string[] Options { get; set; }
      public RelayCommand AcceptCommand { get; private set; }
      public string FilterName { get; set; }
   }
}
