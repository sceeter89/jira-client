using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace JiraAssistant.Logic.Converters
{
   public class BoolToVisibleConverter : IValueConverter
   {
      public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
      {
         if (value is bool == false) return null;

         return (bool) value ? Visibility.Visible : Visibility.Collapsed;
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
      {
         throw new NotImplementedException();
      }
   }
}
