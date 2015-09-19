using GalaSoft.MvvmLight;
using Yakuza.JiraClient.Properties;

namespace Yakuza.JiraClient.Service
{
   public class Configuration : ViewModelBase
   {
      private bool _isLoggedIn;

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
         }
      }

      public bool IsLoggedIn
      {
         get
         { return _isLoggedIn; }
         set
         {
            _isLoggedIn = value;
            RaisePropertyChanged();
         }
      }
   }
}
