using GalaSoft.MvvmLight.Command;
using JiraAssistant.Model.Jira;
using JiraAssistant.Model.Ui;
using JiraAssistant.Services;
using System.Collections.Generic;
using System.Windows.Input;
using System;
using System.Linq;
using System.Windows.Media.Imaging;
using Autofac;
using Telerik.Windows.Data;
using JiraAssistant.Dialogs;
using System.Text;
using System.Text.RegularExpressions;

namespace JiraAssistant.Pages
{
   public partial class BrowseIssuesPage
   {
      private readonly INavigator _navigator;
      private readonly IContainer _iocContainer;

      public BrowseIssuesPage(IList<JiraIssue> issues, IContainer iocContainer)
      {
         InitializeComponent();

         Issues = new QueryableCollectionView(issues);
         _iocContainer = iocContainer;
         _navigator = _iocContainer.Resolve<INavigator>();
         Buttons.Add(new ToolbarButton
         {
            Tooltip = "Scrum Cards",
            Command = new RelayCommand(OpenScrumCards),
            Icon = new BitmapImage(new Uri(@"pack://application:,,,/;component/Assets/Icons/ScrumCard.png"))
         });
         Buttons.Add(new ToolbarButton
         {
            Tooltip = "Export as text",
            Command = new RelayCommand(ExportAsTextResults),
            Icon = new BitmapImage(new Uri(@"pack://application:,,,/;component/Assets/Icons/TxtFormatIcon.png"))
         });
         Buttons.Add(new ToolbarButton
         {
            Tooltip = "Export as rich text",
            Command = new RelayCommand(ExportAsConfluenceMarkupResults),
            Icon = new BitmapImage(new Uri(@"pack://application:,,,/;component/Assets/Icons/ConfluenceIcon.png"))
         });

         DataContext = this;
      }

      private void ExportAsConfluenceMarkupResults()
      {
         var resultBuilder = new StringBuilder();
         var grouped = Issues.OfType<JiraIssue>().GroupBy(i => i.EpicName);

         foreach (var group in grouped)
         {
            if (string.IsNullOrWhiteSpace(group.Key))
               resultBuilder.AppendLine("h2. (No Epic)");
            else
               resultBuilder.AppendLine(group.Key);

            foreach (var issue in group)
               resultBuilder.AppendLine(string.Format("* *{0}* - {1}", issue.Key, EscapeConfluenceMarkupCharacters(issue.Summary)));
         }

         var dialog = new TextualPreview(resultBuilder.ToString());
         dialog.ShowDialog();
      }

      private string EscapeConfluenceMarkupCharacters(string summary)
      {
         return Regex.Replace(summary, @"[{\[\]\}\(\)!@\\]", m => "\\" + m.Value);
      }

      private void ExportAsTextResults()
      {
         var resultBuilder = new StringBuilder();
         var grouped = Issues.OfType<JiraIssue>().GroupBy(i => i.EpicName);

         foreach (var group in grouped)
         {
            if (string.IsNullOrWhiteSpace(group.Key))
               resultBuilder.AppendLine("(No Epic)");
            else
               resultBuilder.AppendLine(group.Key);

            foreach (var issue in group)
               resultBuilder.AppendLine(string.Format("* {0} - {1}", issue.Key, issue.Summary));
         }

         var dialog = new TextualPreview(resultBuilder.ToString());
         dialog.ShowDialog();
      }

      private void OpenScrumCards()
      {
         _navigator.NavigateTo(new ScrumCardsPrintPreview(Issues.OfType<JiraIssue>().ToList(), _iocContainer));
      }

      private void ItemDoubleClick(object sender, MouseButtonEventArgs e)
      {
         if (SelectedIssue != null)
            _navigator.NavigateTo(new IssueDetailsPage(SelectedIssue));
      }

      public QueryableCollectionView Issues { get; private set; }
      public JiraIssue SelectedIssue { get; set; }
      public override string Title { get { return "Issues browser"; } }
   }
}
