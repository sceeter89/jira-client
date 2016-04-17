using JiraAssistant.Model.Ui;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace JiraAssistant.Pages
{
   public abstract class BaseNavigationPage : UserControl, INavigationPage, INotifyPropertyChanged
   {
      public BaseNavigationPage()
      {
         Buttons = new ObservableCollection<IToolbarItem>();
      }

      public ObservableCollection<IToolbarItem> Buttons
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

      public abstract string Title
      {
         get;
      }

      public event PropertyChangedEventHandler PropertyChanged;

      public virtual void OnNavigatedFrom()
      { }

      public virtual void OnNavigatedTo()
      { }

      protected void RaisePropertyChanged([CallerMemberName]string memberName = null)
      {
         if (PropertyChanged != null)
            PropertyChanged(this, new PropertyChangedEventArgs(memberName));
      }
   }
}
