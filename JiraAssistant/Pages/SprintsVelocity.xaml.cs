using JiraAssistant.Model.Jira;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Telerik.Charting;

namespace JiraAssistant.Pages
{
   public partial class SprintsVelocity
   {
      private readonly IDictionary<int, IList<JiraIssue>> _sprintsIssues;
      private readonly IList<RawAgileSprint> _sprints;

      public SprintsVelocity(IDictionary<int, IList<JiraIssue>> sprintsIssues, IList<RawAgileSprint> sprints)
      {
         InitializeComponent();
         _sprintsIssues = sprintsIssues;
         _sprints = sprints;
         VelocityPerSprint = new ObservableCollection<CategoricalDataPoint>();
         CommitmentPerSprint = new ObservableCollection<CategoricalDataPoint>();

         LoadData();

         DataContext = this;
      }

      private void LoadData()
      {
         foreach (var sprint in _sprints.OrderBy(s => s.StartDate))
         {
            var commitment = _sprintsIssues[sprint.Id].Sum(i => i.StoryPoints);
            var completed = _sprintsIssues[sprint.Id].Where(i => i.Resolved.HasValue && i.Resolved <= sprint.CompleteDate).Sum(i => i.StoryPoints);

            VelocityPerSprint.Add(new CategoricalDataPoint { Category = sprint.Name, Value = completed });
            CommitmentPerSprint.Add(new CategoricalDataPoint { Category = sprint.Name, Value = commitment });
         }
      }

      public ObservableCollection<CategoricalDataPoint> VelocityPerSprint { get; private set; }
      public ObservableCollection<CategoricalDataPoint> CommitmentPerSprint { get; private set; }
   }
}
