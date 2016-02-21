using JiraAssistant.Model.Jira;
using JiraAssistant.Services;
using System.Collections.Generic;
using System.Windows.Input;

namespace JiraAssistant.Pages
{
   public partial class BrowseIssuesPage
   {
      private readonly INavigator _navigator;

      public BrowseIssuesPage(IList<JiraIssue> issues, INavigator navigator)
      {
         InitializeComponent();

         Issues = issues;
         _navigator = navigator;

         DataContext = this;
      }

      private void ItemDoubleClick(object sender, MouseButtonEventArgs e)
      {
         if (SelectedIssue != null)
            _navigator.NavigateTo(new IssueDetailsPage(SelectedIssue));
      }

      public IList<JiraIssue> Issues { get; private set; }
      public JiraIssue SelectedIssue { get; set; }
   }
}
