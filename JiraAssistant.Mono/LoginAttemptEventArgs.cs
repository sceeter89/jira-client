using System;
namespace JiraAssistant.Mono
{
	public class LoginAttemptEventArgs : EventArgs
	{
		public LoginAttemptEventArgs(string jiraUrl, string username, string password)
		{
			JiraUrl = jiraUrl;
			Username = username;
			Password = password;
		}

		public string JiraUrl { get; private set; }
		public string Password { get; private set; }
		public string Username { get; private set; }
	}
		
}
