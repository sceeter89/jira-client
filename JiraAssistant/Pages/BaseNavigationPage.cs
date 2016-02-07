using JiraAssistant.Model.Ui;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace JiraAssistant.Pages
{
   public class BaseNavigationPage : UserControl, INavigationPage, INotifyPropertyChanged
   {
      public BaseNavigationPage()
      {
         Buttons = new ObservableCollection<ToolbarButton>();
      }

      public ObservableCollection<ToolbarButton> Buttons
      {
         get; private set;
      }

      public Control Control
      {
         get { return this; }
      }

      public Control StatusBarControl
      {
         get; protected set;
      }

      public event PropertyChangedEventHandler PropertyChanged;

      public virtual void OnNavigatedFrom()
      {

      }

      public virtual void OnNavigatedTo()
      {

      }

      protected void RaisePropertyChanged([CallerMemberName]string memberName = null)
      {
         if (PropertyChanged != null)
            PropertyChanged(this, new PropertyChangedEventArgs(memberName));
      }
   }
}
