using GalaSoft.MvvmLight.Command;
using System.ComponentModel.Composition;
using System.Reflection;
using System.Windows.Input;
using Yakuza.JiraClient.Api;
using Yakuza.JiraClient.Api.Messages.Actions;
using Yakuza.JiraClient.Api.Messages.IO.Exports;
using Yakuza.JiraClient.Messaging.Api;

namespace Yakuza.JiraClient.Plugins.Diagnostics
{
   [Export(typeof(IMicroservice))]
   public class DiagnosticsMicroservice : IMicroservice
   {
      private IMessageBus _messageBus;

      public ICommand ExportLogCommand { get; internal set; }
      public RelayCommand CheckForUpdatesCommand { get; private set; }

      public DiagnosticsMicroservice()
      {
         ExportLogCommand = new RelayCommand(() => _messageBus.Send(new SaveLogOutputToFileMessage()));
         CheckForUpdatesCommand = new RelayCommand(() => _messageBus.Send(new CheckForUpdatesMessage(Assembly.GetEntryAssembly().GetName().Version)));
      }

      public void Initialize(IMessageBus messageBus)
      {
         _messageBus = messageBus;
      }
   }
}
