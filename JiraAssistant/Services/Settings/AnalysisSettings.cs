namespace JiraAssistant.Services.Settings
{
   public class AnalysisSettings : SettingsBase
   {
      public int NumberOfLastSprintsAnalysed
      {
         get { return GetValue(15); }
         set { SetValue(value, 15); }
      }
   }
}
