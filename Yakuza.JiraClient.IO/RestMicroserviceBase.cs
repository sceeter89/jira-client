using RestSharp;
using System.Net;
using Yakuza.JiraClient.Api;
using Yakuza.JiraClient.Messaging.Api;

namespace Yakuza.JiraClient.IO
{
   public class RestMicroserviceBase
   {
      protected readonly IConfiguration _configuration;
      protected readonly IMessageBus _messageBus;

      public RestMicroserviceBase(IConfiguration configuration, IMessageBus messageBus)
      {
         _configuration = configuration;
         _messageBus = messageBus;
      }

      protected RestClient BuildRestClient()
      {
         var client = new RestClient(_configuration.JiraUrl);
         client.AddDefaultHeader("Content-Type", "Application/json");
         if (string.IsNullOrEmpty(_configuration.JiraSessionId) == false)
         {
            client.CookieContainer = new CookieContainer();
            client.CookieContainer.Add(new Cookie("JSESSIONID", _configuration.JiraSessionId, "/", client.BaseUrl.Host));
         }

         return client;
      }

      protected bool IsConfigValid()
      {
         return string.IsNullOrWhiteSpace(_configuration.JiraUrl) == false;
      }
   }
}
