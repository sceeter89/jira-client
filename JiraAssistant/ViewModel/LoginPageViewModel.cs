using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using JiraAssistant.Pages;
using JiraAssistant.Services;
using System.Windows.Input;

namespace JiraAssistant.ViewModel
{
   public class LoginPageViewModel : ViewModelBase
   {
      private readonly INavigator _navigator;

      public LoginPageViewModel(INavigator navigator)
      {
         _navigator = navigator;
      }

      public ICommand LoginCommand { get { return new RelayCommand(Login); } }

      private void Login()
      {
         _navigator.NavigateTo(new SelectProjectPage());
      }
   }
}
