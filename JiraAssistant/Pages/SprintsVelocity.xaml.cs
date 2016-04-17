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
         Minimum = new ObservableCollection<SprintStatistic>();
         Maximum = new ObservableCollection<SprintStatistic>();
         Average = new ObservableCollection<SprintStatistic>();

         LoadData();

         DataContext = this;
      }

      private void LoadData()
      {
         var toSkip = _sprints.Count - _settings.NumberOfLastSprintsAnalysed;
         var sprints = _sprints.Where(s => s.CompleteDate.HasValue).OrderBy(s => s.StartDate).Skip(toSkip);
         foreach (var sprint in sprints)
         {
            var commitment = _sprintsIssues[sprint.Id].Sum(i => i.StoryPoints);
            var completed = _sprintsIssues[sprint.Id].Where(i => i.Resolved.HasValue && i.Resolved <= sprint.CompleteDate).Sum(i => i.StoryPoints);

            Statistics.Add(new SprintStatistic { SprintName = sprint.Name, Velocity = completed, Commitment = commitment });
         }

         var minimumVelocity = Statistics.Min(s => s.Velocity);
         var maximumVelocity = Statistics.Max(s => s.Velocity);
         var averageVelocity = Statistics.Average(s => s.Velocity);
         var minimumCommitment = Statistics.Min(s => s.Commitment);
         var maximumCommitment = Statistics.Max(s => s.Commitment);
         var averageCommitment = Statistics.Average(s => s.Commitment);
         Minimum.Add(new SprintStatistic { SprintName = Statistics.First().SprintName, Velocity = minimumVelocity, Commitment = minimumCommitment });
         Minimum.Add(new SprintStatistic { SprintName = Statistics.Last().SprintName, Velocity = minimumVelocity, Commitment = minimumCommitment });
         Maximum.Add(new SprintStatistic { SprintName = Statistics.First().SprintName, Velocity = maximumVelocity, Commitment = maximumCommitment });
         Maximum.Add(new SprintStatistic { SprintName = Statistics.Last().SprintName, Velocity = maximumVelocity, Commitment = maximumCommitment });
         Average.Add(new SprintStatistic { SprintName = Statistics.First().SprintName, Velocity = averageVelocity, Commitment = averageCommitment });
         Average.Add(new SprintStatistic { SprintName = Statistics.Last().SprintName, Velocity = averageVelocity, Commitment = averageCommitment });
      }

      public ObservableCollection<SprintStatistic> Statistics { get; private set; }
      public ObservableCollection<SprintStatistic> Minimum { get; private set; }
      public ObservableCollection<SprintStatistic> Maximum { get; private set; }
      public ObservableCollection<SprintStatistic> Average { get; private set; }
      public override string Title { get { return "Sprints' velocity"; } }

      public class SprintStatistic
      {
         public string SprintName { get; set; }
         public double Velocity { get; set; }
         public double Commitment { get; set; }
      }
   }
}
