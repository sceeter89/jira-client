using GalaSoft.MvvmLight.Command;

namespace JiraAssistant.Dialogs
{
   public partial class FilterNameDialog
   {
      public FilterNameDialog()
      {
         InitializeComponent();
         AcceptCommand = new RelayCommand(() => DialogResult = true);
         DataContext = this;
      }

      public RelayCommand AcceptCommand { get; private set; }
      public string FilterName { get; set; }
   }
}
