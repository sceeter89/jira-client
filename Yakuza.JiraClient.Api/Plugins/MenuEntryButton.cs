using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Yakuza.JiraClient.Api.Plugins
{
   public class MenuEntryButton
   {
      public string Label { get; set; }
      public BitmapImage Icon { get; set; }
      public ICommand OnClickCommand { get; set; }
   }
}