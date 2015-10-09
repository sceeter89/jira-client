using System;
using System.Windows.Media.Imaging;
using Yakuza.JiraClient.Messaging.Api;

namespace Yakuza.JiraClient.Api.Plugins
{
   public class MenuEntryButton
   {
      public string Label { get; set; }
      public BitmapImage Icon { get; set; }
      public Action<IMessageBus> OnClick { get; set; }
   }
}