using JiraAssistant.Model.Jira;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace JiraAssistant.Pages
{
   public partial class EpicsOverviewPage
   {
      private readonly IEnumerable<JiraIssue> _issues;
      private readonly IDictionary<string, string> _epicKeyToName;
      private readonly IDictionary<string, bool> _epicKeyToIgnoreStatus;
      private bool _isBusy;

      public EpicsOverviewPage(IEnumerable<JiraIssue> issues, IEnumerable<RawAgileEpic> epics)
      {
         InitializeComponent();

         _issues = issues;

         _epicKeyToIgnoreStatus = epics.ToDictionary(e => e.Key, e => e.Done);
         _epicKeyToIgnoreStatus[""] = false;
         _epicKeyToName = epics.ToDictionary(e => e.Key, e => e.Name);
         _epicKeyToName[""] = "(No epic)";
         ChartItems = new ObservableCollection<EpicShare>();

         DataContext = this;
         GenerateData();
      }

      private async void GenerateData()
      {
         IsBusy = true;
         var newItems = await Task.Factory.StartNew(() =>
               _issues
                  .Where(i => _epicKeyToIgnoreStatus[i.EpicLink] == false)
                  .GroupBy(i => _epicKeyToName[i.EpicLink])
                  .Select(group => new EpicShare(group.Key, group.Count())));

         ChartItems.Clear();

         foreach (var item in newItems)
            ChartItems.Add(item);

         IsBusy = false;
      }

      public ObservableCollection<EpicShare> ChartItems { get; private set; }
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

   public class EpicShare
   {
      public EpicShare(string epicName, int issuesCount)
      {
         EpicName = epicName;
         IssuesCount = issuesCount;
      }

      public string EpicName { get; private set; }
      public double IssuesCount { get; private set; }
   }
}
