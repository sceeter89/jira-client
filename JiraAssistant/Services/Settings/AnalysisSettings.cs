namespace JiraAssistant.Services.Settings
{
   public class AnalysisSettings : SettingsBase
   {
      public int NumberOfLastSprintsAnalysed
      {
         get { return (int)GetValue(15L); }
         set { SetValue(value, 15L); }
      }
   }
}
