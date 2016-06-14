using JiraAssistant.Domain.Ui;

namespace JiraAssistant.Domain.NavigationMessages
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
