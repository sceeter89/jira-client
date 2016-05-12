using Microsoft.Win32;
using System.Reflection;

namespace JiraAssistant.Settings
{
   public class GeneralSettings : SettingsBase
   {
      public bool EnableUpdates
      {
         get { return GetValue(defaultValue: true); }
         set { SetValue(value, defaultValue: true); }
      }

      public bool InformAboutUpdate
      {
         get { return GetValue(defaultValue: false); }
         set { SetValue(value, defaultValue: false); }
      }

      public bool OnlyStableVersions
      {
         get { return GetValue(defaultValue: true); }
         set { SetValue(value, defaultValue: true); }
      }

      public bool RunOnWindowsStart
      {
         get { return GetValue(defaultValue: false); }
         set
         {
            SetValue(value, defaultValue: false);
            var key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (value)
               key.SetValue("Jira Assistant", Assembly.GetExecutingAssembly().Location);
            else
               key.DeleteValue("Jira Assistant", false);
         }
      }

      public bool CloseToTray
      {
         get { return GetValue(defaultValue: false); }
         set { SetValue(value, defaultValue: false); }
      }
   }
}
