using JiraAssistant.Model.Jira;
using System;
using System.Threading.Tasks;

namespace JiraAssistant.Services.Jira
{
   public interface IJiraSessionApi
   {
      Task<RawProfileDetails> GetProfileDetails();
      Task Logout();
      Task<bool> CheckJiraSession();
      Task AttemptLogin(string jiraUrl, string username, string password);
      event EventHandler OnSuccessfulLogin;
      event EventHandler OnLogout;
   }
}