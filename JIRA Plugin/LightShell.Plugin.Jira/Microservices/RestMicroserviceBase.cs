using LightShell.Api;
using LightShell.Messaging.Api;
using LightShell.Plugin.Jira.Api;
using RestSharp;
using System;
using System.Net;

namespace LightShell.Plugin.Jira.Microservices
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
         return string.IsNullOrWhiteSpace(_configuration.JiraUrl) == false
            && (_configuration.JiraUrl.StartsWith("http://") || _configuration.JiraUrl.StartsWith("https://"));
      }
   }
}
