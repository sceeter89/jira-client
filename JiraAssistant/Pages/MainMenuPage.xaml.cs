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

      public ObservableCollection<IToolbarItem> Buttons
      {
         get { return new ObservableCollection<IToolbarItem>(); }
      }

      public Control Control
      {
         get { return this; }
      }

      public Control StatusBarControl
      {
         get { return null; }
      }

      public void OnNavigatedFrom() { }

      public void OnNavigatedTo()
      {
         var viewModel = DataContext as MainMenuViewModel;
         viewModel.OnNavigatedTo();
      }

      public string Title { get { return "Main menu"; } }
   }
}
