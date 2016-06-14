using JiraAssistant.Domain.Ui;

namespace JiraAssistant.Logic.Services
{
   public interface INavigator
   {
      void NavigateTo(INavigationPage pane);
      void Back();
      void ClearHistory();
   }
}
