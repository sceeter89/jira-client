using JiraAssistant.Domain.Ui;
using System.Windows.Data;
using System.Windows.Media;
using System;
using System.Globalization;

namespace JiraAssistant.Controls.Converters
{
    public class ColorInfoToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var info = value as ColorInfo;

            if (info == null)
                return null;

            return new SolidColorBrush(new Color
            {
                A = info.Alpha,
                R = info.R,
                G = info.G,
                B = info.B
            });
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
