using System;

namespace JiraAssistant.Logic.Settings
{
   public class AssistantSettings : SettingsBase
   {
      public string JiraUrl
      {
         get { return GetValue(defaultValue: string.Empty); }

         set
         {
            if (value.StartsWith("http") == false)
               value = "https://" + value;

            SetValue(value, defaultValue: string.Empty);
         }
      }

      public string Username
      {
         get { return GetValue(defaultValue: string.Empty); }
         set { SetValue(value, defaultValue: string.Empty); }
      }

      public string RecentBoardsIds
      {
         get { return GetValue(defaultValue: string.Empty); }
         set { SetValue(value, defaultValue: string.Empty); }
      }

      public DateTime LastLogin
      {
         get { return GetValue(defaultValue: DateTime.MinValue); }
         set { SetValue(value, defaultValue: DateTime.MinValue); }
      }

      public string SessionCookies
      {
         get { return GetValue(defaultValue: string.Empty); }
         set { SetValue(value, defaultValue: string.Empty); }
      }
   }
}
