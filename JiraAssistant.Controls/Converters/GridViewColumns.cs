using JiraAssistant.Domain.Ui;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Telerik.Windows.Controls;

namespace JiraAssistant.Controls.Converters
{
    public class GridViewColumns : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var columnInfos = value as IEnumerable<GridColumnInfo>;
            if (columnInfos == null)
                return null;

            return new ObservableCollection<GridViewDataColumn>(columnInfos.Select(i => new GridViewDataColumn
            {
                Header = i.Header,
                DataMemberBinding = new Binding(i.PropertyName) { Converter = i.ApplySecondsToHoursConverter ? new SecondsToHoursConverter() : null },
                IsReadOnly = true
            }));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
