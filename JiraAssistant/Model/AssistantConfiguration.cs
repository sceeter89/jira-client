using GalaSoft.MvvmLight;
using JiraAssistant.Properties;

namespace JiraAssistant.Model
{
   public class AssistantConfiguration : ViewModelBase
   {
      public string JiraUrl
      {
         get
         {
            return Settings.Default.JiraUrl;
         }
         set
         {
            if (value.StartsWith("http") == false)
               JiraUrl = "https://" + value;
            else
            {
               Settings.Default.JiraUrl = value;
               Settings.Default.Save();
               RaisePropertyChanged();
            }
         }
      }

      public string Username
      {
         get
         {
            return Settings.Default.Username;
         }
         set
         {
            Settings.Default.Username = value;
            Settings.Default.Save();
            RaisePropertyChanged();
         }
      }

      public string JiraSessionId
      {
         get
         {
            return Settings.Default.JiraSessionId;
         }
         set
         {
            Settings.Default.JiraSessionId = value;
            Settings.Default.Save();
            RaisePropertyChanged();
         }
      }
   }
}
