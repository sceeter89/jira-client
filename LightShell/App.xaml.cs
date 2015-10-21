using GalaSoft.MvvmLight.Threading;

namespace LightShell
{
   public partial class App
   {
      static App()
      {
         DispatcherHelper.Initialize();
      }
   }
}
