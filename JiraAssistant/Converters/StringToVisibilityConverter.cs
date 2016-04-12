using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace JiraAssistant.Converters
{
   public class StringToVisibilityConverter : IValueConverter
   {
      public ImageSource TrueIcon { get; set; }
      public ImageSource FalseIcon { get; set; }

      public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
      {
         var message = value as string;

         return string.IsNullOrWhiteSpace(message) ? Visibility.Collapsed : Visibility.Visible;
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
      {
         throw new NotImplementedException();
      }
   }
}
