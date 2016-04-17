using Autofac;
using GalaSoft.MvvmLight.Command;
using JiraAssistant.Model.Jira;
using JiraAssistant.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace JiraAssistant.Pages
{
   public partial class EpicsOverviewPage
   {
      private readonly IEnumerable<JiraIssue> _issues;
      private readonly IDictionary<string, string> _epicKeyToName;
      private readonly IDictionary<string, bool> _epicKeyToIgnoreStatus;
      private bool _isBusy;
      private readonly INavigator _navigator;
      private readonly IContainer _iocContainer;

      public EpicsOverviewPage(IEnumerable<JiraIssue> issues, IEnumerable<RawAgileEpic> epics, IContainer iocContainer)
      {
         InitializeComponent();

         _issues = issues;
         _navigator = iocContainer.Resolve<INavigator>();
         _iocContainer = iocContainer;
         _epicKeyToIgnoreStatus = epics.ToDictionary(e => e.Key, e => e.Done);
         _epicKeyToIgnoreStatus[""] = false;
         _epicKeyToName = epics.ToDictionary(e => e.Key, e => e.Name);
         _epicKeyToName[""] = "(No epic)";
         EpicsStatistics = new ObservableCollection<EpicShare>();

         DataContext = this;
         GenerateData();
      }

      private async void GenerateData()
      {
         IsBusy = true;
         var newItems = await Task.Factory.StartNew(() =>
               _issues
                  .Where(i => _epicKeyToIgnoreStatus[i.EpicLink] == false && i.Resolved.HasValue == false)
                  .GroupBy(i => _epicKeyToName[i.EpicLink])
                  .Select(group => new EpicShare(group.Key, group.ToList())));

         EpicsStatistics.Clear();

         foreach (var item in newItems)
            EpicsStatistics.Add(item);

         IsBusy = false;
      }

      public EpicShare SelectedEpic { get; set; }
      public ObservableCollection<EpicShare> EpicsStatistics { get; private set; }
      public bool IsBusy
      {
         get { return _isBusy; }
         set
         {
            _isBusy = value;
            RaisePropertyChanged();
         }
      }

      private void ItemDoubleClick(object sender, MouseButtonEventArgs e)
      {
         if (SelectedEpic != null)
            _navigator.NavigateTo(new BrowseIssuesPage(SelectedEpic.Issues, _iocContainer));
      }

      public RelayCommand<IList<JiraIssue>> ShowIssuesCommand { get; private set; }
      public override string Title { get { return "Epics overview"; } }
   }

   public class EpicShare
   {
      public EpicShare(string epicName, IList<JiraIssue> issues)
      {
         EpicName = epicName;
         IssuesCount = issues.Count();
         Issues = issues;
      }

      public string EpicName { get; private set; }
      public IList<JiraIssue> Issues { get; private set; }
      public double IssuesCount { get; private set; }
   }
}
