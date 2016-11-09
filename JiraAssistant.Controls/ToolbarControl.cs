using JiraAssistant.Domain.Ui;
using System.Windows.Controls;

namespace JiraAssistant.Controls
{
   public class ToolbarControl : IToolbarItem
   {
      public Control Control { get; set; }
   }
}
