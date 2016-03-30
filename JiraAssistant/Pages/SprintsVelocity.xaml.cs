using Autofac;
using JiraAssistant.Model.Jira;
using JiraAssistant.Services.Settings;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace JiraAssistant.Pages
{
   public partial class SprintsVelocity
   {
      private readonly IDictionary<int, IList<JiraIssue>> _sprintsIssues;
      private readonly IList<RawAgileSprint> _sprints;
      private readonly AnalysisSettings _settings;

      public SprintsVelocity(IDictionary<int, IList<JiraIssue>> sprintsIssues, IList<RawAgileSprint> sprints, IContainer iocContainer)
      {
         InitializeComponent();
         _sprintsIssues = sprintsIssues;
         _sprints = sprints;
         _settings = iocContainer.Resolve<AnalysisSettings>();
         Statistics = new ObservableCollection<SprintStatistic>();

         LoadData();

         DataContext = this;
      }

      private void LoadData()
      {
         var toSkip = _sprints.Count - _settings.NumberOfLastSprintsAnalysed;
         foreach (var sprint in _sprints.OrderBy(s => s.StartDate).Skip(toSkip))
         {
            var commitment = _sprintsIssues[sprint.Id].Sum(i => i.StoryPoints);
            var completed = _sprintsIssues[sprint.Id].Where(i => i.Resolved.HasValue && i.Resolved <= sprint.CompleteDate).Sum(i => i.StoryPoints);

            Statistics.Add(new SprintStatistic { SprintName = sprint.Name, Velocity = completed, Commitment = commitment });
         }
      }

      public ObservableCollection<SprintStatistic> Statistics { get; private set; }

      public class SprintStatistic
      {
         public string SprintName { get; set; }
         public double Velocity { get; set; }
         public double Commitment { get; set; }
      }
   }
}
