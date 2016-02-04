using JiraAssistant.Model.Ui;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using JiraAssistant.Model.Jira;

namespace JiraAssistant.Pages
{
   public partial class AgileBoardPage : INavigationPage
   {
      private readonly RawAgileBoard _board;

      public AgileBoardPage(RawAgileBoard board)
      {
         InitializeComponent();
         _board = board;
      }

      public ObservableCollection<ToolbarButton> Buttons
      {
         get { return new ObservableCollection<ToolbarButton>(); }
      }

      public Control Control
      {
         get { return this; }
      }

      public void OnNavigatedFrom()
      {
      }

      public void OnNavigatedTo()
      {

      }
   }
}
