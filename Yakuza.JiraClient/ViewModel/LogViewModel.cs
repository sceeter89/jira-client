using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Threading;
using System;
using Yakuza.JiraClient.Api.Messages.Actions;
using Yakuza.JiraClient.Messaging.Api;

namespace Yakuza.JiraClient.ViewModel
{
   public class LogViewModel : ViewModelBase,
      IHandleMessage<LogMessage>
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

      public ObservableCollection<string> Messages { get; private set; }
   }
}