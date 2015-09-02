using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using JiraManager.Messages.Actions;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Threading;

namespace JiraManager.ViewModel
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
            Messages.Insert(0, message.Message);
         });
      }

      public ObservableCollection<string> Messages { get; private set; }
   }
}