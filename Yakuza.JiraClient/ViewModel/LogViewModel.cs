using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Threading;
using System;
using Yakuza.JiraClient.Api.Messages.Actions;
using Yakuza.JiraClient.Messaging.Api;
using Yakuza.JiraClient.Api.Messages.IO.Exports;
using System.IO;

namespace Yakuza.JiraClient.ViewModel
{
   public class LogViewModel : ViewModelBase,
      IHandleMessage<LogMessage>,
      IHandleMessage<SaveLogOutputToFileMessage>
   {
      public LogViewModel(IMessageBus messenger)
      {
         Messages = new ObservableCollection<string>();
         messenger.Register(this);
      }

      public void Handle(LogMessage message)
      {
         DispatcherHelper.CheckBeginInvokeOnUI(() =>
         {
            Messages.Insert(0, string.Format("[{0}][{1}] {2}", DateTime.Now, message.Level, message.Message));
         });
      }

      public void Handle(SaveLogOutputToFileMessage message)
      {
         var dlg = new Microsoft.Win32.SaveFileDialog();
         dlg.FileName = DateTime.Today.ToString("yyyyMMdd") + " Jira Client Log.txt";
         dlg.DefaultExt = ".txt";
         dlg.Filter = "TXT File (.txt)|*.txt";
         dlg.OverwritePrompt = true;

         var result = dlg.ShowDialog();

         if (result == false)
            return;

         var filename = dlg.FileName;
         if (File.Exists(filename))
            File.Delete(filename);

         using (var fileWriter = new StreamWriter(filename))
         {
            for (int i = Messages.Count - 1; i >= 0; i--)
               fileWriter.WriteLine(Messages[i]);
         }
      }

      public ObservableCollection<string> Messages { get; private set; }
   }
}