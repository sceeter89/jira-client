using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using JiraManager.Service;

namespace JiraManager.ViewModel
{
   public class LoginViewModel : ViewModelBase
   {
      private readonly Configuration _configuration;
      private readonly IMessenger _messenger;
      private string _password;

      public LoginViewModel(IMessenger messenger, Configuration configuration)
      {
         _messenger = messenger;
         _configuration = configuration;
      }

      public string JiraUrl
      {
         get { return _configuration.JiraUrl; }
         set
         {
            _configuration.JiraUrl = value;
            RaisePropertyChanged();
         }
      }

      public string Username
      {
         get { return _configuration.Username; }
         set
         {
            _configuration.Username = value;
            RaisePropertyChanged();
         }
      }

      public string Password
      {
         get { return _password; }
         set
         {
            _password = value;
            RaisePropertyChanged();
         }
      }
   }
}