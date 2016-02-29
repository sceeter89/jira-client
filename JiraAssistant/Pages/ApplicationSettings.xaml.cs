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
   }

   public enum SettingsPage
   {
      Updates = 0,
      Graveyard = 1
   }
}
