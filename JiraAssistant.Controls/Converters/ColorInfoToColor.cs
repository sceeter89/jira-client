using JiraAssistant.Domain.Ui;
using System.Windows.Data;
using System.Windows.Media;
using System;
using System.Globalization;

namespace JiraAssistant.Controls.Converters
{
    public class ColorInfoToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var info = value as ColorInfo;

            if (info == null)
                return null;

            return new Color
            {
                A = info.Alpha,
                R = info.R,
                G = info.G,
                B = info.B
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public static class ColorInfoToColor
    {
        public static Color ToColor(this ColorInfo info)
        {
            return new Color
            {
                A = info.Alpha,
                R = info.R,
                G = info.G,
                B = info.B
            };
        }
        public static ColorInfo ToColorInfo(this Color color)
        {
            return new ColorInfo
            {
                Alpha = color.A,
                R = color.R,
                G = color.G,
                B = color.B
            };
        }
    }
}
