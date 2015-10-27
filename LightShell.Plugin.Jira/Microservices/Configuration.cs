using LightShell.Plugin.Jira.Api;
using LightShell.Plugin.Jira.Properties;

namespace LightShell.Plugin.Jira.Microservices
{
   public class Configuration : IConfiguration
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
   }
}
