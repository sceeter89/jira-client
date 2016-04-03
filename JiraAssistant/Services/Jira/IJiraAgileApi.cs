using JiraAssistant.Model.Jira;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JiraAssistant.Services.Jira
{
   public interface IJiraAgileApi
   {
      Task<RawAgileSprint> GetAgileSprintDetails(int sprintId);
      Task<IEnumerable<RawAgileSprint>> GetSprints(int boardId);
      Task<IEnumerable<string>> GetIssuesInSprint(int boardId, int sprintId);
      Task<IEnumerable<RawAgileEpic>> GetEpics(int boardId);
      Task<IEnumerable<RawAgileBoard>> GetAgileBoards();
      Task<RawAgileBoardConfiguration> GetBoardConfiguration(int boardId);
   }
}
