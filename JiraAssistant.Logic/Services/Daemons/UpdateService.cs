using System;
using RestSharp;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using RestSharp.Extensions;
using System.IO;
using System.Diagnostics;
using NLog;
using JiraAssistant.Logic.Settings;
using JiraAssistant.Domain.Github;
using JiraAssistant.Logic.ContextlessViewModels;
using System.Reflection;
using JiraAssistant.Logic.Extensions;
using JiraAssistant.Domain.Ui;
using GalaSoft.MvvmLight.Messaging;
using JiraAssistant.Domain.Messages.Dialogs;
using JiraAssistant.Domain.Messages;
using System.Timers;

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
        private bool _inProgress;
        private readonly Timer _timer;
        private readonly IMessenger _messenger;

        public UpdateService(GeneralSettings settings, MainViewModel mainViewModel, IMessenger messenger)
        {
            _settings = settings;
            _messenger = messenger;
            _mainViewModel = mainViewModel;

            _messenger.Register<PerformApplicationUpdateMessage>(this, PerformApplicationUpdate);

            AppDomain.CurrentDomain.ProcessExit += OnApplicationExit;
            
            _timer = new Timer();
            _timer.Elapsed += CheckForUpdates;
            _timer.Interval = TimeSpan.FromMinutes(20).TotalMilliseconds;
            _timer.Start();
        }
        
        private void OnApplicationExit(object sender, EventArgs e)
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

        private void PerformApplicationUpdate(PerformApplicationUpdateMessage message)
        {
            _runInstaller = true;
            if (message.Method == UpdateMethod.ExitAndInstall)
            {                
                _showInstallerUi = true;
                _messenger.Send(new ShutdownApplicationMessage());
                return;
            }
            _mainViewModel.UserMessage = string.Format("New version will be installed once you close application.");
        }

        private async Task DownloadToFile(string url, string destinationPath)
        {
            var client = new RestClient(url);
            var request = new RestRequest("/");
            await Task.Factory.StartNew(() => client.DownloadData(request).SaveAs(destinationPath));
        }

        private async void CheckForUpdates(object sender, EventArgs args)
        {
            if (_inProgress)
                return;
            _inProgress = true;

            if (_settings.EnableUpdates == false)
                return;
            try
            {
                var client = new RestClient(EndpointUrl);
                var request = new RestRequest("/", Method.GET);

                var currentVersion = Assembly.GetEntryAssembly().GetName().Version;

                var response = await client.ExecuteGetTaskAsync(request);
                var releases = await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<List<GithubApplicationRelease>>(response.Content));
                if (releases == null)
                    return;

                var higherVersions = releases.Where(r => r.Draft == false
                                                      && (_settings.OnlyStableVersions == false || r.Prerelease != _settings.OnlyStableVersions)
                                                      && Version.Parse(r.TagName) > currentVersion)
                                                  .OrderByDescending(r => r.TagName);

                if (higherVersions.Any() == false)
                    return;

                _timer.Enabled = false;

                var higherVersion = higherVersions.First();
                
                var installer = higherVersion.Assets.First(a => a.Name.EndsWith(".msi"));

                _installerPath = Path.Combine(Path.GetTempPath(), installer.Name);

                if (File.Exists(_installerPath) == false || new FileInfo(_installerPath).Length != installer.Size)
                    await DownloadToFile(installer.BrowserDownloadUrl, _installerPath);

                if (_settings.InformAboutUpdate || higherVersion.Prerelease)
                {
                    _messenger.Send(new OpenUpdateAvailableDialogMessage(
                        currentVersion,
                        Version.Parse(higherVersion.TagName),
                        higherVersion.Prerelease == false,
                        _installerPath
                        ));
                }
            }
            catch (Exception e)
            {
                Sentry.CaptureException(e);
                _logger.Warn(e, "Failed to check for updates.");
            }
            finally
            {
                _inProgress = false;
            }
        }
    }
}
