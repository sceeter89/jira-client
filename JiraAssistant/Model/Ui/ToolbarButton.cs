using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace JiraAssistant.Model.Ui
{
   public class ToolbarButton
   {
      public string Tooltip { get; set; }
      public BitmapSource Icon { get; set; }
      public ICommand Command { get; set; }
   }
}
