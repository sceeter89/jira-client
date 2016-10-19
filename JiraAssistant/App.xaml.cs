using System;
using GalaSoft.MvvmLight.Threading;
using NLog.Config;
using NLog.Targets;
using System.IO;
using NLog;
using SharpRaven;

namespace JiraAssistant
{
    public partial class App
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public App()
        {
            ConfigureLogger();

            InitializeComponent();
            DispatcherHelper.Initialize();

        }
        private void ConfigureLogger()
        {
            var config = new LoggingConfiguration();

            var consoleTarget = new ColoredConsoleTarget();
            config.AddTarget("console", consoleTarget);
            var logsDirPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                                                           "Yakuza", "Jira Assistant", "Logs");

            if (Directory.Exists(logsDirPath) == false)
                Directory.CreateDirectory(logsDirPath);

            var fileTarget = new FileTarget
            {
                Name = "main",
                FileName = Path.Combine(logsDirPath, "Jira Assistant.log"),
                Layout = "${longdate} ${uppercase:${level}} ${message} ${exception:format=tostring}",
                ArchiveFileName = Path.Combine(logsDirPath, "Jira Assistant.{#}.log"),
                ArchiveEvery = FileArchivePeriod.Day,
                ArchiveNumbering = ArchiveNumberingMode.Rolling,
                MaxArchiveFiles = 3,
                ConcurrentWrites = true
            };
            config.AddTarget("file", fileTarget);

            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, consoleTarget));
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Info, fileTarget));

            LogManager.Configuration = config;
        }
    }
}
