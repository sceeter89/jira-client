using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Threading;
using LightShell.Api;
using LightShell.Api.Messages.IO.Plugins;
using LightShell.Api.Plugins;
using LightShell.Messaging.Api;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace LightShell.Plugin.Diagnostics.Controls
{
   public class PluginsViewModel : ViewModelBase,
      IMicroservice,
      IHandleMessage<NewPluginFoundMessage>
   {
      private PluginInfo _selectedPlugin;

      public void Handle(NewPluginFoundMessage message)
      {
         DispatcherHelper.CheckBeginInvokeOnUI(() =>
         {
            Plugins.Add(PreparePluginInfo(message.PluginDescription));
         });
      }

      private PluginInfo PreparePluginInfo(ILightShellPlugin plugin)
      {
         var companyAttribute = plugin.GetType().Assembly.GetCustomAttribute<AssemblyCompanyAttribute>();

         return new PluginInfo
         {
            Name = plugin.PluginName,
            Version = plugin.GetType().Assembly.GetName().Version,
            Vendor = companyAttribute == null ? "N/A" : companyAttribute.Company,
            FilePath = plugin.GetType().Assembly.Location,

            PluginStructure = new[] {
               new PluginStructureElement
               {
                  Name = "Plugin structure",
                  Children = new[]
                  {
                     new PluginStructureElement
                     {
                        Name = "Microservices",
                        Children = (plugin.GetMicroservices() ?? Enumerable.Empty<IMicroservice>())
                                       .Select(m => new PluginStructureElement {Name = m.GetType().Name})
                     },
                     new PluginStructureElement
                     {
                        Name = "Menu entries",
                        Children = (plugin.GetMenuEntries() ?? Enumerable.Empty<MenuEntryDescriptor>())
                                       .SelectMany(m => m.Buttons.Select(b =>
                                          new PluginStructureElement { Name=string.Format("{0} » {1} » {2}", m.Tab, m.ButtonsGroupName, b.Label) }))
                     },
                  }
               }
            }
         };
      }

      public void Initialize(IMessageBus messageBus)
      {
         DispatcherHelper.CheckBeginInvokeOnUI(() =>
         {
            Plugins = new ObservableCollection<PluginInfo>();
         });
         messageBus.Register(this);
      }

      public ObservableCollection<PluginInfo> Plugins { get; private set; }
      public PluginInfo SelectedPlugin
      {
         get { return _selectedPlugin; }
         set
         {
            _selectedPlugin = value;
            RaisePropertyChanged();
         }
      }

      public class PluginStructureElement
      {
         public string Name { get; set; }
         public IEnumerable<PluginStructureElement> Children { get; set; }
      }

      public class PluginInfo
      {
         public string Name { get; set; }
         public Version Version { get; set; }
         public string Vendor { get; set; }
         public string FilePath { get; set; }

         public IEnumerable<PluginStructureElement> PluginStructure { get; set; }
      }
   }
}
