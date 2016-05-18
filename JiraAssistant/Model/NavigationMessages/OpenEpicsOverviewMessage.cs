using JiraAssistant.Model.Jira;
using System.Collections.Generic;

namespace JiraAssistant.Model.NavigationMessages
{
   public class OpenEpicsOverviewMessage
   {
      public OpenEpicsOverviewMessage(IList<JiraIssue> issues, IList<RawAgileEpic> epics)
      {
         Issues = issues;
         Epics = epics;
      }

      public IList<RawAgileEpic> Epics { get; private set; }
      public IList<JiraIssue> Issues { get; private set; }
   }
}
