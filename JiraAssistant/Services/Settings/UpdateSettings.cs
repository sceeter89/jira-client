namespace JiraAssistant.Services.Settings
{
   public class UpdateSettings : SettingsBase
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
   }
}
