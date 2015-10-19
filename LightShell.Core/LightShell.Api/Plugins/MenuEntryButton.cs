using System;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using LightShell.Messaging.Api;

namespace LightShell.Api.Plugins
{
   public class MenuEntryButton
   {
      public string Label { get; set; }
      public BitmapImage Icon { get; set; }
      public Action<IMessageBus> OnClickDelegate { get; set; }
      public ICommand OnClickCommand { get; set; }
   }
}