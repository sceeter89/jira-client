using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace JiraAssistant.Controls.Converters
{
   public class BoolToIconConverter : IValueConverter
   {
      public ImageSource TrueIcon { get; set; }
      public ImageSource FalseIcon { get; set; }

      public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
      {
         if (value is bool == false)
            return null;

         var flag = (bool)value;

         return flag ? TrueIcon : FalseIcon;
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
      {
         throw new NotImplementedException();
      }
   }
}
