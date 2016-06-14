using JiraAssistant.Domain.Jira;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Media;

namespace JiraAssistant.Logic.Services.Jira
{
   public interface IJiraApi
   {
      Task<ImageSource> DownloadPicture(string imageUri);

      Task<IList<JiraIssue>> SearchForIssues(string jqlQuery);

      IJiraAgileApi Agile { get; }
      IJiraServerApi Server { get; }
      IJiraSessionApi Session { get; }
      IJiraWorklogManager Worklog { get; }
   }
}