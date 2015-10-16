using RestSharp;
using System.Net;
using Yakuza.JiraClient.Api;
using Yakuza.JiraClient.Messaging.Api;

namespace Yakuza.JiraClient.IO
{
   public class RestMicroserviceBase : IMicroservice
   {
      protected readonly IConfiguration _configuration;
      protected IMessageBus _messageBus;

      public RestMicroserviceBase(IConfiguration configuration)
      {
         _configuration = configuration;
      }

      public void Initialize(IMessageBus messageBus)
      {
         _messageBus = messageBus;
         _messageBus.Register(this);
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
