using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JiraAssistant.Domain.Jira
{
   public interface IJiraAgileApi
   {
      Task<RawAgileSprint> GetAgileSprintDetails(int sprintId);
      Task<IEnumerable<RawAgileSprint>> GetSprints(int boardId);
      Task<IEnumerable<RawAgileEpic>> GetEpics(int boardId);
      Task<IEnumerable<RawAgileBoard>> GetAgileBoards();
      Task<RawAgileBoardConfiguration> GetBoardConfiguration(int boardId);
      Task<AgileBoardIssues> GetBoardContent(int boardId, bool forceReload = false, Action<float> progressUpdateCallback = null);
   }
}
