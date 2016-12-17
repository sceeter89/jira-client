using RestSharp;
using System.Net;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using System.Linq;
using JiraAssistant.Logic.Settings;
using JiraAssistant.Domain.Jira;
using JiraAssistant.Domain.Exceptions;

namespace JiraAssistant.Logic.Services.Resources
{
    public class JiraSessionService : BaseRestService, IJiraSessionApi
    {
        public JiraSessionService(AssistantSettings configuration)
           : base(configuration)
        {
        }

        public event EventHandler OnSuccessfulLogin;
        public event EventHandler OnLogout;

        public async Task<RawProfileDetails> GetProfileDetails()
        {
            var client = BuildRestClient();
            var request = new RestRequest("/rest/api/latest/myself?expand=groups,applicationRoles", Method.GET);

            var response = await client.ExecuteTaskAsync(request);
            var result = JsonConvert.DeserializeObject<RawProfileDetails>(response.Content);

            return result;
        }

        public async Task Logout()
        {
            var client = BuildRestClient();

            var response = await client.ExecuteTaskAsync(new RestRequest("/rest/auth/1/session", Method.DELETE));
            Configuration.SessionCookies = "";

            RaiseOnLogout();
        }

        private void RaiseOnLogout()
        {
            if (OnLogout != null)
                OnLogout(this, EventArgs.Empty);
        }

        private void RaiseOnSuccessfulLogin()
        {
            if (OnSuccessfulLogin != null)
                OnSuccessfulLogin(this, EventArgs.Empty);
        }

        public async Task<bool> CheckJiraSession()
        {
            try
            {
                var client = BuildRestClient();

                var response = await client.ExecuteGetTaskAsync<RawSessionInfo>(new RestRequest("/rest/auth/1/session"));

                if (response.StatusCode == 0)
                {
                    throw new ServerNotFoundException();
                }

                if (response.StatusCode == HttpStatusCode.Unauthorized || response.Data == null)
                {
                    Configuration.SessionCookies = "";
                    return false;
                }

                RaiseOnSuccessfulLogin();
                return true;
            }
            catch (UriFormatException)
            {
                return false;
            }
        }

        public async Task AttemptLogin(string jiraUrl, string username, string password)
        {
            try
            {
                Configuration.JiraUrl = jiraUrl;
                Configuration.Username = username;
                var client = BuildRestClient();

                var sessionInfoRequest = new RestRequest("/rest/gadget/1.0/login");
                sessionInfoRequest.AddParameter("os_username", username, ParameterType.GetOrPost);
                sessionInfoRequest.AddParameter("os_password", password, ParameterType.GetOrPost);
                sessionInfoRequest.AddParameter("os_cookie", "true", ParameterType.GetOrPost);

                var response = await client.ExecutePostTaskAsync(sessionInfoRequest);

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new LoginFailedException("Invalid username or password");
                }

                if (response.StatusCode == HttpStatusCode.Forbidden)
                {
                    throw new LoginFailedException("User was not allowed to log in. Try to login via browser.");
                }

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    if (response.StatusCode == 0)
                        throw new ServerNotFoundException();

                    throw new LoginFailedException("Server returned unexpected response code: " + response.StatusCode);
                }

                if (string.IsNullOrEmpty(response.Content))
                {
                    throw new LoginFailedException(string.Format("Given address '{0}' does not point at valid JIRA server.", jiraUrl));
                }

                var loginResult = JsonConvert.DeserializeObject<RawLoginResult>(response.Content);

                if (loginResult.LoginSucceeded == false)
                    throw new LoginFailedException(LoginResultToReason(loginResult));

                Configuration.SessionCookies = response.Headers.First(h => h.Name.ToLowerInvariant() == "set-cookie").Value.ToString();
                RaiseOnSuccessfulLogin();
            }
            catch (UriFormatException)
            {
                throw new LoginFailedException(string.Format("Invalid JIRA server address."));
            }
        }

        private string LoginResultToReason(RawLoginResult loginResult)
        {
            if (loginResult.CaptchaFailure)
                return "JIRA needs to verify captcha to unlock your account. Try to login via WWW first.";

            if (loginResult.CommunicationError)
                return "Encountered communication issues.";

            if (loginResult.LoginFailedByPermissions)
                return "Login was correct, but you are not allowed to access JIRA.";

            if (loginResult.LoginError)
                return "Technical issues interrupted login.";

            return "Invalid user name and/or password.";
        }
    }
}
