using System;

namespace JiraAssistant.Logic.Settings
{
   public class GraveyardSettings : SettingsBase
   {
      public TimeSpan UpdateMoreThanBefore
      {
         get { return GetValue(TimeSpan.FromDays(60)); }
         set { SetValue(value, TimeSpan.FromDays(60)); }
      }

      public TimeSpan CreatedMoreThanBefore
      {
         get { return GetValue(TimeSpan.FromDays(90)); }
         set { SetValue(value, TimeSpan.FromDays(90)); }
      }

      public TimeSpan ArchaicCreatedMoreThanBefore
      {
         get { return GetValue(TimeSpan.FromDays(180)); }
         set { SetValue(value, TimeSpan.FromDays(180)); }
      }
   }
}
