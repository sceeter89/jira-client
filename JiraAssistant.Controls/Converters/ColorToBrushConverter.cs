using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace JiraAssistant.Controls.Converters
{
    public class ColorToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Color == false)
                return null;

            var color = (Color) value;

            return new SolidColorBrush(color);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
