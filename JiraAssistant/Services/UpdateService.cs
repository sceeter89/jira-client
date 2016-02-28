using JiraAssistant.Services.Settings;
using System.Windows;
using System;
using RestSharp;
using System.Threading.Tasks;
using Newtonsoft.Json;
using JiraAssistant.Model.Github;
using System.Collections.Generic;
using System.Linq;
using JiraAssistant.Dialogs;

namespace JiraAssistant.Services
{
   public class UpdateService
   {
      private const string EndpointUrl = "https://api.github.com/repos/sceeter89/jira-client/releases";
      private readonly UpdateSettings _settings;

      public UpdateService(UpdateSettings settings)
      {
         _settings = settings;

         Application.Current.Exit += OnApplicationExit;

         CheckForUpdates();
      }

      private async void CheckForUpdates()
      {
         var client = new RestClient(EndpointUrl);
         var request = new RestRequest("/", Method.GET);

         var response = await client.ExecuteGetTaskAsync(request);
         var releases = await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<List<GithubApplicationRelease>>(response.Content));
         var higherVersions = releases.Where(r => r.Draft == false
                                               && r.Prerelease != _settings.OnlyStableVersions
                                               && Version.Parse(r.TagName) > GetType().Assembly.GetName().Version)
                                           .OrderByDescending(r => r.TagName);

         if (higherVersions.Any() == false)
            return;

         if(_settings.InformAboutUpdate)
         {
            var dialog = new UpdateInstallPrompt();
         }


         _messageBus.LogMessage("New version is available. Visit website for download.", LogLevel.Info);
         var newRelease = higherVersions.First();
         _messageBus.Send(new NewVersionAvailable(
            Version.Parse(newRelease.tag_name),
            newRelease.assets.First(a => a.name.EndsWith(".msi")).browser_download_url,
            newRelease.body
            ));

      }

      private void OnApplicationExit(object sender, ExitEventArgs e)
      {

      }
   }
}
