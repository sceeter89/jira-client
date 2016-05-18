using JiraAssistant.Model.Ui;

namespace JiraAssistant.Model.NavigationMessages
{
   public class OpenPageMessage
   {
      public INavigationPage Page { get; private set; }

      public OpenPageMessage(INavigationPage page)
      {
         Page = page;
      }
   }
}
