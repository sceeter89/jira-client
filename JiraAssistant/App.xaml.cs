using GalaSoft.MvvmLight.Threading;

namespace JiraAssistant
{
   public partial class App
   {
      static App()
      {
         if (JiraAssistant.Properties.Settings.Default.SettingsUpgradePending)
         {
            JiraAssistant.Properties.Settings.Default.Upgrade();
            JiraAssistant.Properties.Settings.Default.SettingsUpgradePending = false;
            JiraAssistant.Properties.Settings.Default.Save();
         }
      }

      public App()
      {
         this.InitializeComponent();
         DispatcherHelper.Initialize();
      }
   }
}
