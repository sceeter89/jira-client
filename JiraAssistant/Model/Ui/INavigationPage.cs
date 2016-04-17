using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace JiraAssistant.Model.Ui
{
   public interface INavigationPage
   {
      Control Control { get; }
      Control StatusBarControl { get; }
      ObservableCollection<IToolbarItem> Buttons { get; }
      string Title { get; }
      void OnNavigatedTo();
      void OnNavigatedFrom();
   }
}
