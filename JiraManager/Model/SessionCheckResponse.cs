namespace JiraManager.Model
{
   public class SessionCheckResponse
   {
      public bool IsLoggedIn { get; set; }
      public RawSessionInfo SessionInfo { get; set; }
   }
}
