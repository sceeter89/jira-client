using JiraAssistant.Model.Jira;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JiraAssistant.Services.Jira
{
   public interface IJiraServerApi
   {
      Task<IEnumerable<RawProjectInfo>> GetProjects();
      Task<IEnumerable<RawIssueType>> GetIssueTypes();
      Task<IEnumerable<RawFieldDefinition>> GetFieldsDefinitions();
      Task<IEnumerable<RawPriority>> GetPriorities();
      Task<IEnumerable<RawResolution>> GetResolutions();
      Task<IEnumerable<RawStatus>> GetStatuses();
      Task<RawFilterDefinition> GetFilterDefinition(int filterId);
   }
}
