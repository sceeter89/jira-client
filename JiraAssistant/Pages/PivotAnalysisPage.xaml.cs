using JiraAssistant.Model.Jira;
using System.Collections.Generic;
using Telerik.Pivot.Core;
using System.Threading.Tasks;
using System.Linq;

namespace JiraAssistant.Pages
{
   public partial class PivotAnalysisPage
   {
      private readonly IEnumerable<JiraIssue> _issues;
      private bool _isBusy;

      public PivotAnalysisPage(IEnumerable<JiraIssue> issues)
      {
         InitializeComponent();
         _issues = issues;

         DataSource = new LocalDataSourceProvider();

         DataContext = this;

         LoadData();
      }

      private async void LoadData()
      {
         IsBusy = true;

         var list = await Task.Factory.StartNew(() => _issues.Select(x => new PivotJiraIssue(x)).ToList());

         using (DataSource.DeferRefresh())
         {
            DataSource.ItemsSource = list;
         }

         IsBusy = false;
      }

      public LocalDataSourceProvider DataSource { get; private set; }
      public bool IsBusy
      {
         get { return _isBusy; }
         set
         {
            _isBusy = value;
            RaisePropertyChanged();
         }
      }
   }
}
