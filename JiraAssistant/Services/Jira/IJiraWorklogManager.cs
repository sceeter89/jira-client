using JiraAssistant.Model.Jira;
using System.Threading.Tasks;

namespace JiraAssistant.Services.Jira
{
   public interface IJiraWorklogManager
   {
      Task Log(JiraIssue issue, float hoursSpent);
   }
}