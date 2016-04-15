using System.Windows;
using Telerik.Windows.Documents.Model;

namespace JiraAssistant.Dialogs
{
   public partial class RichTextPreview
   {
      public RichTextPreview(RadDocument document)
      {
         InitializeComponent();

         textBox.Document = document;
      }
      private void ConfirmClick(object sender, RoutedEventArgs e)
      {
         DialogResult = true;
      }
   }
}
