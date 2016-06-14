using System;
using System.Globalization;
using System.Windows.Data;

namespace JiraAssistant.Controls.Converters
{
    public class SecondsToHoursConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var seconds = (long) value;
                var timeSpan = TimeSpan.FromSeconds(seconds);
                return string.Format("{0:0.00}", timeSpan.TotalHours);
            }
            catch { return null; }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
