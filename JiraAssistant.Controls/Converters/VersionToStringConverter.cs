using System;
using System.Globalization;
using System.Windows.Data;

namespace JiraAssistant.Controls.Converters
{
    public class VersionToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int partsCount;
            try
            {
                partsCount = System.Convert.ToInt32(parameter);
            }
            catch
            {
                partsCount = 4;
            }

            if (value is Version == false)
                return null;

            return ((Version)value).ToString(partsCount);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
