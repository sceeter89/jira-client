using System.Collections.ObjectModel;
using System.Windows.Controls;
using JiraAssistant.Model.Ui;
using JiraAssistant.ViewModel;

namespace JiraAssistant.Pages
{
   public partial class MainMenuPage : INavigationPage
   {
      public MainMenuPage()
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

      public void OnNavigatedFrom()
      {
         
      }

      public void OnNavigatedTo()
      {
         var viewModel = DataContext as MainMenuViewModel;
         viewModel.OnNavigatedTo();
      }
   }
}
