namespace JiraAssistant.Domain
{
   public class IssuesCollectionStatistics
   {
      public int IssuesCount { get; set; }
      public int ResolvedIssuesCount { get; set; }
      public int UnresolvedIssuesCount { get; set; }

      public double TotalStorypoints { get; set; }
      public double AverageStorypointsPerTask { get; set; }
      
      public int DistinctReporters { get; set; }

      public double AverageResolutionTimeHours { get; set; }
      public double MaxResolutionTimeHours { get; set; }

      public int EpicsInvolved { get; set; }

      public double AverageSubtasksCount { get; set; }
   }
}
