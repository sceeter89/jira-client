using JiraAssistant.Model.Ui;
using System.Windows.Controls;
using System.Collections.ObjectModel;

namespace JiraAssistant.Pages
{
   public partial class LoginPage : INavigationPage
   {
      public LoginPage()
      {
         InitializeComponent();
      }

      public ObservableCollection<ToolbarButton> Buttons
      {
         get
         {
            return new ObservableCollection<ToolbarButton>();
         }
      }

      public Control Control
      {
         get
         {
            return this;
         }
      }
   }
}
