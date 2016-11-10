using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace JiraAssistant.Controls.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public Visibility HiddenValue { get; set; }

        public BoolToVisibilityConverter()
        {
            HiddenValue = Visibility.Collapsed;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool == false) return null;

            return (bool) value ? Visibility.Visible : HiddenValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
