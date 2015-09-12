using JiraManager.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JiraManager.Api
{
   public interface IJiraOperations
   {
      Task<SessionCheckResponse> CheckSession();
      Task<LoginAttemptResult> TryToLogin(string username, string password);
      Task Logout();
      Task<IEnumerable<RawIssue>> GetIssues(string jql);
      Task<IEnumerable<RawFieldDefinition>> GetFieldDefinitions();
      Task<IEnumerable<RawAgileBoard>> GetAgileBoards();
      Task<IEnumerable<RawAgileSprint>> GetSprintsForBoard(int boardId);
   }
}
