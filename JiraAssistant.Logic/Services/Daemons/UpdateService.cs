using System.Windows;
using System;
using RestSharp;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using JiraAssistant.Controls.Dialogs;
using RestSharp.Extensions;
using Microsoft.Win32;
using System.IO;
using System.Diagnostics;
using NLog;
using JiraAssistant.Logic.ViewModels;
using JiraAssistant.Logic.Settings;
using JiraAssistant.Domain.Github;

namespace JiraAssistant.Logic.Services.Daemons
{
    public class UpdateService
   {
      private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
      private const string EndpointUrl = "https://api.github.com/repos/sceeter89/jira-client/releases";
      private readonly GeneralSettings _settings;
      private bool _runInstaller;
      private bool _showInstallerUi;
      private string _installerPath;
      private readonly MainViewModel _mainViewModel;

      public UpdateService(GeneralSettings settings, MainViewModel mainViewModel)
      {
         _settings = settings;

         Application.Current.Exit += OnApplicationExit;
         _mainViewModel = mainViewModel;

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
                                                  && (_settings.OnlyStableVersions == false || r.Prerelease != _settings.OnlyStableVersions)
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
            {
               _showInstallerUi = true;
               Application.Current.Shutdown();
               return;
            }

            _mainViewModel.UserMessage = string.Format("New version ({0}) will be installed once you close application.", higherVersion.TagName);
         }
         catch (Exception e)
         {
            _logger.Warn(e, "Failed to check for updates.");
         }
      }

      private void OnApplicationExit(object sender, ExitEventArgs e)
      {
         if (_runInstaller == false)
            return;

         var processInfo = new ProcessStartInfo
         {
            FileName = _installerPath,
            UseShellExecute = true,
            Arguments = _showInstallerUi ? "" : "/quiet /passive"
         };
         Process.Start(processInfo);
      }
   }
}
