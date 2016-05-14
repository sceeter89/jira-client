using System.Windows;
using System.Windows.Input;
using Telerik.Windows.Documents.Model;

namespace JiraAssistant.Dialogs
{
   public partial class RichTextPreview
   {
      public RichTextPreview(RadDocument document)
      {
         InitializeComponent();

         textBox.Document = document;

         var mousePosition = App.Current.MainWindow.PointToScreen(Mouse.GetPosition(null));
         this.Top = mousePosition.Y;
         this.Left = mousePosition.X;
      }
      private void ConfirmClick(object sender, RoutedEventArgs e)
      {
         DialogResult = true;
      }
   }
}
