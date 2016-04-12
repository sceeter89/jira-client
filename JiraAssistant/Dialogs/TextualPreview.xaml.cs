using System.Windows;

namespace JiraAssistant.Dialogs
{
   public partial class TextualPreview
   {
      public TextualPreview(string text, bool readOnly = false)
      {
         InitializeComponent();

         Text = text;
         ReadOnly = readOnly;

         DataContext = this;
      }

      public bool ReadOnly { get; private set; }
      public string Text { get; private set; }

      private void ConfirmClick(object sender, RoutedEventArgs e)
      {
         DialogResult = true;
      }
   }
}
