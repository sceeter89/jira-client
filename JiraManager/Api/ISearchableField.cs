using System.Windows.Controls;

namespace JiraManager.Api
{
   public interface ISearchableField
   {
      bool IsFilled { get; }
      string GetSearchQuery();
      void Clear();
      UserControl EditControl { get; }
   }
}
