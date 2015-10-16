using System;
using LightShell.Messaging.Api;

namespace LightShell.Plugin.Jira.Api.Messages.Actions
{
   public class CheckForUpdatesMessage : IMessage
   {
      public CheckForUpdatesMessage(Version currentVersion, bool includePrereleases = false)
      {
         CurrentVersion = currentVersion;
         IncludePrereleases = includePrereleases;
      }

      public Version CurrentVersion { get; private set; }
      public bool IncludePrereleases { get; private set; }
   }

   public class NoUpdatesAvailable : IMessage
   {
      
   }

   public class NewVersionAvailable : IMessage
   {
      public NewVersionAvailable(Version newVersion, string downloadUrl, string changes)
      {
         NewVersion = newVersion;
         DownloadUrl = downloadUrl;
         Changes = changes;
      }

      public string Changes { get; private set; }
      public string DownloadUrl { get; private set; }
      public Version NewVersion { get; private set; }
   }
}
