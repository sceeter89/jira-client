using System;

namespace JiraAssistant.Services.Settings
{
   public class GraveyardSettings : SettingsBase
   {
      public TimeSpan UpdateMoreThanBefore
      {
         get { return GetValue(TimeSpan.FromDays(60)); }
         set { SetValue(value); }
      }

      public TimeSpan CreatedMoreThanBefore
      {
         get { return GetValue(TimeSpan.FromDays(180)); }
         set { SetValue(value); }
      }

      public TimeSpan ArchaicCreatedMoreThanBefore
      {
         get { return GetValue(TimeSpan.FromDays(180)); }
         set { SetValue(value); }
      }
   }
}
