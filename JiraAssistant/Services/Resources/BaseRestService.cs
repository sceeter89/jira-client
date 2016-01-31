using JiraAssistant.Model;
using RestSharp;
using System.Net;

namespace JiraAssistant.Services.Resources
{
   public class BaseRestService
   {
      public BaseRestService(AssistantConfiguration configuration)
      {
         Configuration = configuration;
      }

      protected RestClient BuildRestClient()
      {
         var client = new RestClient(Configuration.JiraUrl);
         client.AddDefaultHeader("Content-Type", "Application/json");
         if (string.IsNullOrEmpty(Configuration.JiraSessionId) == false)
         {
            client.CookieContainer = new CookieContainer();
            client.CookieContainer.Add(new Cookie("JSESSIONID", Configuration.JiraSessionId, "/", client.BaseUrl.Host));
         }

         return client;
      }

      protected AssistantConfiguration Configuration { get; private set; }
   }
}
