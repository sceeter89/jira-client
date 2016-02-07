using JiraAssistant.Model.Ui;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using JiraAssistant.ViewModel;

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
         get { return this; }
      }

      public Control StatusBarControl
      {
         get { return null; }
      }

      public void OnNavigatedFrom()
      {

      }

      public void OnNavigatedTo()
      {
         var viewModel = DataContext as LoginPageViewModel;
         viewModel.AttemptAutoLogin();
      }
   }
}
