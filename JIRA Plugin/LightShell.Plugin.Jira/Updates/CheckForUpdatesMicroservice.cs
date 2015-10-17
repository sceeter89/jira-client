using RestSharp;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using LightShell.Api;
using LightShell.Messaging.Api;
using LightShell.Plugin.Jira.Api.Messages.Actions;

namespace Yakuza.JiraClient.IO.Updates
{
   public class CheckForUpdatesMicroservice : IMicroservice,
      IHandleMessage<CheckForUpdatesMessage>
   {
      private const string EndpointUrl = "https://api.github.com/repos/sceeter89/jira-client/releases";
      private IMessageBus _messageBus;

      public async void Handle(CheckForUpdatesMessage message)
      {
         var client = new RestClient(EndpointUrl);
         var request = new RestRequest("/", Method.GET);

         var response = await client.ExecuteGetTaskAsync(request);
         var releases = await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<List<GithubApplicationRelease>>(response.Content));
         var higherVersions = releases.Where(r => r.draft == false
                                               && r.prerelease == message.IncludePrereleases
                                               && Version.Parse(r.tag_name) > message.CurrentVersion)
                                           .OrderByDescending(r => r.tag_name);
         if (higherVersions.Any() == false)
         {
            _messageBus.LogMessage("You are using latest version available.", LogLevel.Debug);
            _messageBus.Send(new NoUpdatesAvailable());
         }
         else
         {
            _messageBus.LogMessage("New version is available. Visit website for download.", LogLevel.Info);
            var newRelease = higherVersions.First();
            _messageBus.Send(new NewVersionAvailable(
               Version.Parse(newRelease.tag_name),
               newRelease.assets.First(a => a.name.EndsWith(".msi")).browser_download_url,
               newRelease.body
               ));
         }
      }

      public void Initialize(IMessageBus messageBus)
      {
         _messageBus = messageBus;

         _messageBus.Register(this);
      }
   }
}
