using JiraAssistant.Domain.Ui;

namespace JiraAssistant.Domain.NavigationMessages
{
   public class OpenSettingsMessage
   {
      public SettingsPage InitialPage { get; private set; }

      public OpenSettingsMessage(SettingsPage initialPage = SettingsPage.General)
      {
         InitialPage = initialPage;
      }
   }
}
