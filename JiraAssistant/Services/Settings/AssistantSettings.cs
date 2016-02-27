using System;

namespace JiraAssistant.Services.Settings
{
   public class AssistantSettings : SettingsBase
   {
      public string JiraUrl
      {
         get { return GetValue<string>(); }

         set
         {
            if (value.StartsWith("http") == false)
               value = "https://" + value;

            SetValue(value);
         }
      }

      public string Username
      {
         get { return GetValue<string>(); }
         set { SetValue(value); }
      }

      public string JiraSessionId
      {
         get { return GetValue<string>(); }
         set { SetValue(value); }
      }

      public DateTime LastLogin
      {
         get { return GetValue<DateTime>(); }
         set { SetValue(value); }
      }
   }
}
