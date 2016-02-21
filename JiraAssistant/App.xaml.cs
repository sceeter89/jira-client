using GalaSoft.MvvmLight.Threading;
using JiraAssistant.Properties;

namespace JiraAssistant
{
   public partial class App
   {
      static App()
      {
         if (Settings.Default.SettingsUpgradePending)
         {
            Settings.Default.Upgrade();
            Settings.Default.SettingsUpgradePending = false;
            Settings.Default.Save();
         }
      }

      public App()
      {
         this.InitializeComponent();
         DispatcherHelper.Initialize();
      }
   }
}
