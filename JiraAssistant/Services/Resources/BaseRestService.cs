using JiraAssistant.Model.Exceptions;
using JiraAssistant.Settings;
using RestSharp;
using System.Net;

namespace JiraAssistant.Services.Resources
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
         if (string.IsNullOrEmpty(Configuration.JiraSessionId) == false)
         {
            client.CookieContainer = new CookieContainer();
            client.CookieContainer.Add(new Cookie("JSESSIONID", Configuration.JiraSessionId, "/", client.BaseUrl.Host));
         }

         return client;
      }

      protected AssistantSettings Configuration { get; private set; }
   }
}
