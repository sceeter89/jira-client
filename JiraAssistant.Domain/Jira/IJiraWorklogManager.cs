using System.Threading.Tasks;

namespace JiraAssistant.Domain.Jira
{
   public interface IJiraWorklogManager
   {
      Task Log(JiraIssue issue, double hoursSpent);
   }
}