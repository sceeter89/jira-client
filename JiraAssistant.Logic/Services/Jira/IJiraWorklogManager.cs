using JiraAssistant.Domain.Jira;
using System.Threading.Tasks;

namespace JiraAssistant.Logic.Services.Jira
{
   public interface IJiraWorklogManager
   {
      Task Log(JiraIssue issue, double hoursSpent);
   }
}