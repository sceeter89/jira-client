using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace JiraAssistant.Domain.Jira
{
   public interface IJiraApi
   {
      Task<Bitmap> DownloadPicture(string imageUri);

      Task<IList<JiraIssue>> SearchForIssues(string jqlQuery);

      IJiraAgileApi Agile { get; }
      IJiraServerApi Server { get; }
      IJiraSessionApi Session { get; }
      IJiraWorklogManager Worklog { get; }
   }
}