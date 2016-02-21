using Autofac;
using GalaSoft.MvvmLight;

namespace JiraAssistant.ViewModel
{
   public class MainMenuViewModel : ViewModelBase
   {
      private bool _isBusy;
      private string _busyMessage;
      private readonly IContainer _iocContainer;

      public MainMenuViewModel(IContainer iocContainer)
      {
         _iocContainer = iocContainer;
      }
      

      internal void OnNavigatedTo()
      {
         
      }

      public bool IsBusy
      {
         get { return _isBusy; }
         set
         {
            _isBusy = value;
            RaisePropertyChanged();
         }
      }

      public string BusyMessage
      {
         get { return _busyMessage; }
         set
         {
            _busyMessage = value;
            RaisePropertyChanged();
         }
      }
   }
}
