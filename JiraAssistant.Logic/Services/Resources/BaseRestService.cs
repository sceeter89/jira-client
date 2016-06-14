using JiraAssistant.Domain.Exceptions;
using JiraAssistant.Logic.Settings;
using RestSharp;
using System.Net;

namespace JiraAssistant.Logic.Services.Resources
{
    public class BaseRestService
   {
      public BaseRestService(AssistantSettings configuration)
      {
         Configuration = configuration;
      }

      protected RestClient BuildRestClient()
      {
         if (string.IsNullOrEmpty(Configuration.JiraUrl))
            throw new IncompleteJiraConfiguration();

         var client = new RestClient(Configuration.JiraUrl);
         client.AddDefaultHeader("Content-Type", "Application/json");
         if (string.IsNullOrEmpty(Configuration.SessionCookies) == false)
         {
            client.CookieContainer = new CookieContainer();
            client.CookieContainer.SetCookies(client.BaseUrl, Configuration.SessionCookies);
         }

         return client;
      }

      protected AssistantSettings Configuration { get; private set; }
   }
}
