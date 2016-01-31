using JiraAssistant.Model.Ui;

namespace JiraAssistant.Services
{
   public interface INavigator
   {
      void NavigateTo(INavigationPage pane);
      void Back();
   }
}
