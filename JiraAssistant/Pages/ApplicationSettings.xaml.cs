using JiraAssistant.Domain.Ui;

namespace JiraAssistant.Pages
{
   public partial class ApplicationSettings
   {
      private int _selectedIndex;

      public ApplicationSettings()
      {
         InitializeComponent();

         DataContext = this;
      }

      public ApplicationSettings(SettingsPage initialPage)
         : this()
      {
         SelectedIndex = (int) initialPage;
      }

      public int SelectedIndex
      {
         get { return _selectedIndex; }
         set
         {
            _selectedIndex = value;
            RaisePropertyChanged();
         }
      }
      public override string Title { get { return "Settings"; } }
   }
}
