using System;

namespace JiraAssistant.Domain.Messages.Dialogs
{
    public class OpenUpdateAvailableDialogMessage
    {
        public OpenUpdateAvailableDialogMessage(Version currentVersion, Version availableVersion, bool isStable, string installerPath)
        {
            CurrentVersion = currentVersion;
            AvailableVersion = availableVersion;
            IsStable = isStable;
            InstallerPath = installerPath;
        }

        public Version AvailableVersion { get; private set; }
        public Version CurrentVersion { get; private set; }
        public string InstallerPath { get; private set; }
        public bool IsStable { get; private set; }
    }
}
