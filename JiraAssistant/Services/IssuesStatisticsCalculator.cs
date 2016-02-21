using JiraAssistant.Model;
using JiraAssistant.Model.Jira;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace JiraAssistant.Services
{
   public class IssuesStatisticsCalculator
   {
      public async Task<IssuesCollectionStatistics> Calculate(IList<JiraIssue> issues)
      {
         if(issues == null || issues.Any() == false)
            return null;

         return await Task.Factory.StartNew(() =>
         {
            return new IssuesCollectionStatistics
            {
               IssuesCount = issues.Count(),
               ResolvedIssuesCount = issues.Where(i => i.Resolved.HasValue).Count(),
               UnresolvedIssuesCount = issues.Where(i => i.Resolved.HasValue == false).Count(),

               AverageResolutionTimeHours = issues
                                               .Where(i => i.Resolved.HasValue)
                                               .Select(i => (i.Resolved.Value - i.Created).TotalHours).Average(),
               MaxResolutionTimeHours = issues
                                               .Where(i => i.Resolved.HasValue)
                                               .Select(i => (i.Resolved.Value - i.Created).TotalHours).Max(),

               TotalStorypoints = issues.Sum(i => i.StoryPoints),
               AverageStorypointsPerTask = issues.Average(i => i.StoryPoints),

               AverageSubtasksCount = issues.Average(i => i.Subtasks),

               EpicsInvolved = issues.Select(i => i.EpicLink).Distinct().Count(),

               DistinctReporters = issues.Select(i => i.Reporter).Distinct().Count()
            };
         });
      }
   }
}
