namespace JiraManager.Model
{
   public class JiraIssue
   {
      public string Key { get; internal set; }
      public string Priority { get; internal set; }
      public string Project { get; internal set; }
      public int StoryPoints { get; internal set; }
      public string Summary { get; internal set; }
   }
}
