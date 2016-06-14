using System.Windows;

namespace JiraAssistant.Controls
{
   public partial class IssueCollectionStatisticsControl
   {
      public IssueCollectionStatisticsControl()
      {
         InitializeComponent();
      }

      public static readonly DependencyProperty StatisticsProperty =
         DependencyProperty.Register("Statistics", typeof(string), typeof(IssueCollectionStatisticsControl),
         new PropertyMetadata(null));

      public string Statistics
      {
         get { return GetValue(StatisticsProperty).ToString(); }
         set { SetValue(StatisticsProperty, value); }
      } 
   }
}
