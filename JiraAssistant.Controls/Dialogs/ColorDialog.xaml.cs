using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace JiraAssistant.Controls.Dialogs
{
    public partial class ColorDialog : INotifyPropertyChanged
    {
        private Color _editedColor;
        private static ObservableCollection<Color> _globalColorsHistory = new ObservableCollection<Color>();

        public event PropertyChangedEventHandler PropertyChanged;

        public ColorDialog(Color editedColor)
        {
            InitializeComponent();
            EditedColor = editedColor;
            ColorsHistory = _globalColorsHistory;

            ColorEditor.SelectedColor = EditedColor;
            ColorEditor.SelectedColorChanged += (sender, args) =>
            {
                EditedColor = args.NewValue.HasValue ? args.NewValue.Value : Colors.White;
            };

            var mousePosition = Application.Current.MainWindow.PointToScreen(Mouse.GetPosition(null));
            this.Top = mousePosition.Y;
            this.Left = mousePosition.X;

            DataContext = this;
        }

        public ObservableCollection<Color> ColorsHistory { get; private set; }

        public void SaveClicked(object sender, RoutedEventArgs args)
        {
            if (ColorsHistory.Contains(EditedColor) == false)
                ColorsHistory.Add(EditedColor);

            if (ColorsHistory.Count > 5)
                ColorsHistory.RemoveAt(0);

            DialogResult = true;
        }

        public void CancelClicked(object sender, RoutedEventArgs args)
        {
            DialogResult = false;
        }

        public void HistoryButtonClicked(object sender, RoutedEventArgs args)
        {
            var button = (Button) sender;

            EditedColor = (Color) button.DataContext;
            ColorEditor.SelectedColor = EditedColor;
        }

        public Color EditedColor
        {
            get { return _editedColor; }
            set
            {
                _editedColor = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("EditedColor"));
            }
        }
    }
}
