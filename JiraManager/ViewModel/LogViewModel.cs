using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Yakuza.JiraClient.Messages.Actions;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Threading;
using System;

namespace Yakuza.JiraClient.ViewModel
{
   public class LogViewModel : ViewModelBase
   {
      public LogViewModel(IMessenger messenger)
      {
         Messages = new ObservableCollection<string>();
         messenger.Register<LogMessage>(this, Log);
      }

      private void Log(LogMessage message)
      {
         DispatcherHelper.CheckBeginInvokeOnUI(()=>
         {
            Messages.Insert(0, string.Format("[{0}][{1}] {2}", DateTime.Now, message.Level, message.Message));
         });
      }

      public ObservableCollection<string> Messages { get; private set; }
   }
}