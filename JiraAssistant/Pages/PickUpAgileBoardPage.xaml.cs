using System.Collections.ObjectModel;
using System.Windows.Controls;
using JiraAssistant.Model.Ui;
using JiraAssistant.ViewModel;
using System.Windows.Media.Imaging;
using System;

namespace JiraAssistant.Pages
{
   public partial class PickUpAgileBoardPage : INavigationPage
   {
      public PickUpAgileBoardPage()
      {
         InitializeComponent();
         Buttons = new ObservableCollection<IToolbarItem>
         {
            new ToolbarButton
            {
               Tooltip = "Settings",
               Command = (DataContext as AgileBoardSelectViewModel).OpenSettingsCommand,
               Icon = new BitmapImage(new Uri(@"pack://application:,,,/;component/Assets/Icons/Settings.png"))
            }
         };
      }

      public ObservableCollection<IToolbarItem> Buttons
      {
         get; private set;
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
         var viewModel = DataContext as AgileBoardSelectViewModel;
         viewModel.OnNavigatedTo();
      }
   }
}
