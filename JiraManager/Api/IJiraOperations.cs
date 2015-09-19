using Yakuza.JiraClient.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Yakuza.JiraClient.Api
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
      Task<RawProfileDetails> GetProfileDetails();
      Task<BitmapImage> DownloadPicture(string imageUrl);
      Task<IEnumerable<RawIssueType>> GetIssueTypes();
      Task<IEnumerable<RawProjectInfo>> GetProjectsList();
      Task<IEnumerable<RawPriority>> GetPrioritiesList();
      Task<IEnumerable<RawResolution>> GetResolutionsList();
      Task<IEnumerable<RawStatus>> GetStatusesList();
   }
}
