using System.Collections.ObjectModel;

namespace JiraAssistant.Domain.Ui
{
   public interface INavigationPage
   {
      ObservableCollection<IToolbarItem> Buttons { get; }
      string Title { get; }
      void OnNavigatedTo();
      void OnNavigatedFrom();
   }
}
