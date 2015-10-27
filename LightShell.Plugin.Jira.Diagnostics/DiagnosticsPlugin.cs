using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Reflection;
using System.Windows.Media.Imaging;
using LightShell.Api;
using LightShell.Api.Plugins;
using LightShell.Plugin.Jira.Api.Messages.Actions;

namespace LightShell.Plugin.Jira.Diagnostics
{
   [Export(typeof(ILightShellPlugin))]
   public class DiagnosticsPlugin : ILightShellPlugin
   {
      private const string WebSiteAddress = "https://github.com/sceeter89/jira-client";
      private const string ReportIssueSiteAddress = "https://github.com/sceeter89/jira-client/issues/new";

      public string PluginName
      {
         get
         {
            return "JIRA Plugin - support";
         }
      }

      public IEnumerable<MenuEntryDescriptor> GetMenuEntries()
      {
         yield return new MenuEntryDescriptor
         {
            Tab = "support",
            ButtonsGroupName = "help",
            Buttons = new[]
               {
                  new MenuEntryButton
                  {
                     Label = "website",
                     Icon = new BitmapImage(new Uri(@"pack://application:,,,/Jira Diagnostics Plugin;component/Assets/WebsiteIcon.png")),
                     OnClickDelegate = _ => System.Diagnostics.Process.Start(WebSiteAddress)
                  },
                  new MenuEntryButton
                  {
                     Label = "report",
                     Icon = new BitmapImage(new Uri(@"pack://application:,,,/Jira Diagnostics Plugin;component/Assets/ReportIssueIcon.png")),
                     OnClickDelegate = _ => System.Diagnostics.Process.Start(ReportIssueSiteAddress)
                  }
               }
         };
         yield return new MenuEntryDescriptor
         {
            Tab = "support",
            ButtonsGroupName = "updates",
            Buttons = new[]
               {
                  new MenuEntryButton
                  {
                     Label = "check",
                     Icon = new BitmapImage(new Uri(@"pack://application:,,,/Jira Diagnostics Plugin;component/Assets/CheckForUpdatesIcon.png")),
                     OnClickDelegate = bus => bus.Send(new CheckForUpdatesMessage(Assembly.GetEntryAssembly().GetName().Version))
                  }
               }
         };
      }

      public IEnumerable<IMicroservice> GetMicroservices()
      {
         yield break;
      }
   }
}
