namespace JiraAssistant.Services.Settings
{
   public class UpdateSettings : SettingsBase
   {
      public bool AutomaticallyUpdate
      {
         get { return GetValue(defaultValue: true); }
         set { SetValue(value); }
      }

      public bool InformAboutUpdate
      {
         get { return GetValue(defaultValue: true); }
         set { SetValue(value); }
      }

      public bool OnlyStableVersions
      {
         get { return GetValue(defaultValue: false); }
         set { SetValue(value); }
      }
   }
}
