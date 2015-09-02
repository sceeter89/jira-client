namespace JiraManager.Model
{
   public class SessionCheckResponse
   {
      public bool IsLoggedIn { get; set; }
      public SessionInfo SessionInfo { get; set; }
   }
}
