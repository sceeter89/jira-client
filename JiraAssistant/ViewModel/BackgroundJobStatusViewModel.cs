using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Threading;

namespace JiraAssistant.ViewModel
{
   public class BackgroundJobStatusViewModel : ViewModelBase
   {
      private string _title;
      private string _message;
      private int _progress;

      public BackgroundJobStatusViewModel()
      {
         Stop();
      }

      public string Title
      {
         get
         {
            return _title;
         }
         private set
         {
            _title = value;
            RaisePropertyChanged();
         }
      }

      public string Message
      {
         get
         {
            return _message;
         }
         private set
         {
            _message = value;
            RaisePropertyChanged();
         }
      }

      public int Progress
      {
         get
         {
            return _progress;
         }
         private set
         {
            if (value > 100)
               value = 100;
            if (value < 0)
               value = 0;

            _progress = value;
            RaisePropertyChanged();
         }
      }

      public void StartNewJob(string title)
      {
         DispatcherHelper.CheckBeginInvokeOnUI(() =>
         {
            Title = title;
         });
      }

      public void SetMessage(string message)
      {
         DispatcherHelper.CheckBeginInvokeOnUI(() =>
         {
            Message = message;
         });
      }

      public void Stop()
      {
         DispatcherHelper.CheckBeginInvokeOnUI(() =>
         {
            Title = "Idle";
            Message = string.Empty;
            Progress = 0;
         });
      }
   }
}
