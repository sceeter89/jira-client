using JiraManager.Model;
using System.Threading.Tasks;

namespace JiraManager.Api
{
   public interface IJiraOperations
   {
      Task<SessionCheckResponse> CheckSession();
      Task<LoginAttemptResult> TryToLogin(string username, string password);
      Task Logout();
   }
}
