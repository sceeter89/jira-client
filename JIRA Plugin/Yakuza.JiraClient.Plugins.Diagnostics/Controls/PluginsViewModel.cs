using GalaSoft.MvvmLight;
using LightShell.Api;
using LightShell.Api.Messages.IO.Plugins;
using LightShell.Messaging.Api;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace Yakuza.JiraClient.Plugins.Diagnostics.Controls
{
   public class PluginsViewModel : ViewModelBase,
      IMicroservice,
      IHandleMessage<NewPluginFoundMessage>
   {
      private PluginInfo _selectedPlugin;

      public void Handle(NewPluginFoundMessage message)
      {
         var companyAttribute = message.PluginDescription.GetType().Assembly.GetCustomAttribute<AssemblyCompanyAttribute>();


         Plugins.Add(new PluginInfo
         {
            Name = message.PluginDescription.PluginName,
            Version = message.PluginDescription.GetType().Assembly.GetName().Version,
            Vendor = companyAttribute == null ? "N/A" : companyAttribute.Company,
            FilePath = message.PluginDescription.GetType().Assembly.Location,

            PluginStructure = new[] {
               new PluginStructureElement
               {
                  Name = "Plugin structure",
                  Children = new[]
                  {
                     new PluginStructureElement
                     {
                        Name = "Microservices",
                        Children = message.PluginDescription.GetMicroservices().Select(m => new PluginStructureElement {Name = m.GetType().Name})
                     },
                     new PluginStructureElement
                     {
                        Name = "Menu entries",
                        Children = message.PluginDescription.GetMenuEntries().SelectMany(m => m.Buttons.Select(b =>
                                                                                           new PluginStructureElement {
                                                                                              Name=string.Format("{0} » {1} » {2}",
                                                                                              m.Tab,
                                                                                              m.ButtonsGroupName,
                                                                                              b.Label) }))
                     },
                  }
               }
            }
         });
      }

      public void Initialize(IMessageBus messageBus)
      {
         Plugins = new ObservableCollection<PluginInfo>();

         messageBus.Register(this);

         messageBus.Send(new CheckForUpdatesMessage(Assembly.GetEntryAssembly().GetName().Version));
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
