using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace JiraAssistant.Controls.Converters
{
    public class PasswordExtractor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var passwordBox = value as PasswordBox;

            if (passwordBox == null)
                return null;

            return new Func<string>(() => passwordBox.Password);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
