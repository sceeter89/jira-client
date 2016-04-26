using GalaSoft.MvvmLight.Command;

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
      }

      public string[] Options { get; set; }
      public RelayCommand AcceptCommand { get; private set; }
      public string FilterName { get; set; }
   }
}
