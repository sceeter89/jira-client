using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using JiraAssistant.Domain.Messages;
using JiraAssistant.Domain.Ui;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;

namespace JiraAssistant.Controls.Dialogs
{
    public partial class UpdateInstallPrompt
    {
        public UpdateInstallPrompt(Version current, Version latest, bool isStable, string installerPath, IMessenger messenger)
        {
            InitializeComponent();

            CurrentVersion = current;
            LatestVersion = latest;
            StableVersionNoticeVisibility = isStable ? Visibility.Visible : Visibility.Collapsed;
            PrereleaseVersionNoticeVisibility = isStable ? Visibility.Collapsed : Visibility.Visible;

            InstallOnExitCommand = new RelayCommand(() =>
            {
                messenger.Send(new PerformApplicationUpdateMessage(UpdateMethod.InstallOnExit));
                Close();
            });
            CloseAndInstallCommand = new RelayCommand(() =>
            {
                messenger.Send(new PerformApplicationUpdateMessage(UpdateMethod.ExitAndInstall));
                Close();
            });
            ManualInstallCommand = new RelayCommand(() =>
            {
                var saveDialog = new SaveFileDialog { FileName = Path.GetFileName(installerPath) };
                if (saveDialog.ShowDialog() == true)
                    File.Move(installerPath, saveDialog.FileName);
                Close();
            });
            CancelCommand = new RelayCommand(() =>
            {
                Close();
            });

            DataContext = this;
        }

        public Version CurrentVersion { get; private set; }
        public Version LatestVersion { get; private set; }

        public Visibility StableVersionNoticeVisibility { get; private set; }
        public Visibility PrereleaseVersionNoticeVisibility { get; private set; }
        
        public RelayCommand InstallOnExitCommand { get; private set; }
        public RelayCommand CloseAndInstallCommand { get; private set; }
        public RelayCommand ManualInstallCommand { get; private set; }
        public RelayCommand CancelCommand { get; private set; }
    }
}
