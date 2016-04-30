using JiraAssistant.Model.Jira;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Media;

namespace JiraAssistant.Services.Jira
{
   public interface IJiraApi
   {
      Task<ImageSource> DownloadPicture(string imageUri);

      Task<IEnumerable<JiraIssue>> SearchForIssues(string jqlQuery);

      IJiraAgileApi Agile { get; }
      IJiraServerApi Server { get; }
      IJiraSessionApi Session { get; }
      IJiraWorklogManager Worklog {get; }
   }
}