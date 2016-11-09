using GalaSoft.MvvmLight;
using JiraAssistant.Domain.Jira;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JiraAssistant.Logic.ViewModels
{
    public class PivotAnalysisViewModel : ViewModelBase
    {
        private readonly IEnumerable<JiraIssue> _issues;
        private bool _isBusy;

        public PivotAnalysisViewModel(IList<JiraIssue> issues)
        {
            _issues = issues;

            LoadData();
        }

        private async void LoadData()
        {
            IsBusy = true;

            var list = await Task.Factory.StartNew(() => _issues.Select(x => new PivotJiraIssue(x)).ToList());

            Items = list;
            RaisePropertyChanged(() => Items);

            IsBusy = false;
        }

        public IList<PivotJiraIssue> Items { get; private set; }
        

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
