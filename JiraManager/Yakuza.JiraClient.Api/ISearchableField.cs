using System.Windows.Controls;
using System.Windows.Input;

namespace Yakuza.JiraClient.Api
{
   public interface ISearchableField
   {
      bool IsFilled { get; }
      string GetSearchQuery();
      ICommand ClearCommand { get; }
      UserControl EditControl { get; }
   }
}
