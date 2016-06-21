using JiraAssistant.Domain.Jira;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace JiraAssistant.Controls.Converters
{
    public class RawJiraGroupsToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var groups = value as RawGroups;
            if (groups == null) return "No groups";

            return "Groups: " + string.Join(", ", groups.Items.Select(g => g.Name));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
