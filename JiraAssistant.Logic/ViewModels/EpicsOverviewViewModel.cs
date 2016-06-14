using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using JiraAssistant.Domain.Jira;
using JiraAssistant.Domain.NavigationMessages;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace JiraAssistant.Logic.ViewModels
{
    public class EpicsOverviewViewModel : ViewModelBase
   {
      private readonly IEnumerable<JiraIssue> _issues;
      private readonly IDictionary<string, string> _epicKeyToName;
      private readonly IDictionary<string, bool> _epicKeyToIgnoreStatus;

      private bool _isBusy;
      private readonly IMessenger _messenger;

      public EpicsOverviewViewModel(IEnumerable<JiraIssue> issues, IEnumerable<RawAgileEpic> epics, IMessenger messenger)
      {
         _issues = issues;
         _messenger = messenger;
         _epicKeyToIgnoreStatus = epics.ToDictionary(e => e.Key, e => e.Done);
         _epicKeyToIgnoreStatus[""] = false;
         _epicKeyToName = epics.ToDictionary(e => e.Key, e => e.Name);
         _epicKeyToName[""] = "(No epic)";
         EpicsStatistics = new ObservableCollection<EpicShare>();
         
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

      public void OpenSelectedEpic()
      {
         _messenger.Send(new OpenIssuesBrowserMessage(SelectedEpic.Issues));
      }
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
