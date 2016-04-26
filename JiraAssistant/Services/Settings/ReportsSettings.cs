using System;

namespace JiraAssistant.Services.Settings
{
   public class ReportsSettings : SettingsBase
   {
      public bool RemindAboutWorklog
      {
         get { return GetValue(defaultValue: false); }
         set { SetValue(value, defaultValue: false); }
      }

      public DateTime RemindAt
      {
         get { return GetValue(defaultValue: new DateTime(1, 1, 1, 16, 0, 0)); }
         set { SetValue(value, defaultValue: new DateTime(1, 1, 1, 16, 0, 0)); }
      }
   }
}
