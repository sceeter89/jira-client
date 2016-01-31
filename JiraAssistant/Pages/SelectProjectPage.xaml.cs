using System.Collections.ObjectModel;
using System.Windows.Controls;
using JiraAssistant.Model.Ui;

namespace JiraAssistant.Pages
{
   public partial class SelectProjectPage : INavigationPage
   {
      public SelectProjectPage()
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
