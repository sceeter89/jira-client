using GalaSoft.MvvmLight.Threading;
using Yakuza.JiraClient.Properties;
using System.Windows;

namespace Yakuza.JiraClient
{
   /// <summary>
   /// Interaction logic for App.xaml
   /// </summary>
   public partial class App : Application
   {
      static App()
      {
         DispatcherHelper.Initialize();
         if (Settings.Default.SettingsUpgradePending)
         {
            Settings.Default.Upgrade();
            Settings.Default.SettingsUpgradePending = false;
            Settings.Default.Save();
         }
      }
   }
}
