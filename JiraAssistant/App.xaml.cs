using GalaSoft.MvvmLight.Threading;

namespace JiraAssistant
{
   public partial class App
   {
      public App()
      {
         this.InitializeComponent();
         DispatcherHelper.Initialize();
      }
   }
}
