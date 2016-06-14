using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace JiraAssistant.Domain.Ui
{
   public class ToolbarButton : IToolbarItem
   {
      public string Tooltip { get; set; }
      public BitmapSource Icon { get; set; }
      public ICommand Command { get; set; }
   }
}
