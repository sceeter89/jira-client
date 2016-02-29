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
using RestSharp.Extensions;
using Microsoft.Win32;
using System.IO;
using System.Diagnostics;
using NLog;

namespace JiraAssistant.Services
{
   public class UpdateService
   {
      private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
      private const string EndpointUrl = "https://api.github.com/repos/sceeter89/jira-client/releases";
      private readonly UpdateSettings _settings;
      private bool _runInstaller;
      private string _installerPath;

      public UpdateService(UpdateSettings settings)
      {
         _settings = settings;

         Application.Current.Exit += OnApplicationExit;

         CheckForUpdates();
      }

      private async Task DownloadToFile(string url, string destinationPath)
      {
         var client = new RestClient(url);
         var request = new RestRequest("/");
         await Task.Factory.StartNew(() => client.DownloadData(request).SaveAs(destinationPath));
      }

      private async void CheckForUpdates()
      {
         if (_settings.EnableUpdates == false)
            return;
         try
         {
            var client = new RestClient(EndpointUrl);
            var request = new RestRequest("/", Method.GET);

            var currentVersion = GetType().Assembly.GetName().Version;

            var response = await client.ExecuteGetTaskAsync(request);
            var releases = await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<List<GithubApplicationRelease>>(response.Content));
            var higherVersions = releases.Where(r => r.Draft == false
                                                  && r.Prerelease != _settings.OnlyStableVersions
                                                  && Version.Parse(r.TagName) > currentVersion)
                                              .OrderByDescending(r => r.TagName);

            if (higherVersions.Any() == false)
               return;

            var higherVersion = higherVersions.First();

            var closeApplicationAfterDownload = false;
            var installer = higherVersion.Assets.First(a => a.Name.EndsWith(".msi"));

            _installerPath = Path.Combine(Path.GetTempPath(), installer.Name);

            if (File.Exists(_installerPath) == false || new FileInfo(_installerPath).Length != installer.Size)
               await DownloadToFile(installer.BrowserDownloadUrl, _installerPath);

            if (_settings.InformAboutUpdate)
            {
               var dialog = new UpdateInstallPrompt(currentVersion, Version.Parse(higherVersion.TagName), higherVersion.Prerelease == false);

               var result = dialog.Prompt();

               if (result == UpdatePromptResult.None)
                  return;

               if (result == UpdatePromptResult.InstallManually)
               {
                  var saveDialog = new SaveFileDialog { FileName = installer.Name };
                  if (saveDialog.ShowDialog() == true)
                     File.Copy(_installerPath, saveDialog.FileName);
                  return;
               }
               if (result == UpdatePromptResult.ExitAndInstall)
                  closeApplicationAfterDownload = true;
            }

            _runInstaller = true;
            if (closeApplicationAfterDownload)
               Application.Current.Shutdown();
         }
         catch (Exception e)
         {
            _logger.Warn(e, "Failed to check for updates.");
         }
      }

      private void OnApplicationExit(object sender, ExitEventArgs e)
      {
         if (_runInstaller)
         {
            var processInfo = new ProcessStartInfo
            {
               FileName = _installerPath,
               UseShellExecute = true,
               Arguments = "/quiet /passive"
            };
            Process.Start(processInfo);
         }
      }
   }
}
