using System.Windows;
using System.Windows.Input;

namespace JiraAssistant.Controls.Dialogs
{
   public partial class TextualPreview
   {
      public TextualPreview(string text, bool readOnly = false)
      {
         InitializeComponent();

         Text = text;
         ReadOnly = readOnly;

         DataContext = this;

         var mousePosition = Application.Current.MainWindow.PointToScreen(Mouse.GetPosition(null));
         this.Top = mousePosition.Y;
         this.Left = mousePosition.X;
      }

      public bool ReadOnly { get; private set; }
      public string Text { get; private set; }

      private void ConfirmClick(object sender, RoutedEventArgs e)
      {
         DialogResult = true;
      }
   }
}
