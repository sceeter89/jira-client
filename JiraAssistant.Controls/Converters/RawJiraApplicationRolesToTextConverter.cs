using JiraAssistant.Domain.Jira;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace JiraAssistant.Controls.Converters
{
    public class RawJiraApplicationRolesToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var roles = value as RawApplicationRoles;
            if (roles == null) return "No roles";

            return "Roles: " + string.Join(", ", roles.Items.Select(g => g.Name));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
