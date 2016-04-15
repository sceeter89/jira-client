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
using Telerik.Windows.Documents.Model;
using Telerik.Windows.Documents.Model.Styles;
using Telerik.Windows.Documents.Layout;
using System.Windows.Media;
using Telerik.Windows.Documents.Lists;

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
            Command = new RelayCommand(ExportAsRichTextResults),
            Icon = new BitmapImage(new Uri(@"pack://application:,,,/;component/Assets/Icons/RtfFormatIcon.png"))
         });

         DataContext = this;
      }

      private void ExportAsRichTextResults()
      {
         var document = new RadDocument();
         var editor = new RadDocumentEditor(document);
         
         var grouped = Issues.OfType<JiraIssue>().GroupBy(i => i.EpicName);

         foreach (var group in grouped)
         {
            editor.InsertParagraph();
            editor.ChangeStyleName(RadDocumentDefaultStyles.GetHeadingStyleNameByIndex(1));
            if (string.IsNullOrWhiteSpace(group.Key))
               editor.Insert("(No epic)");
            else
               editor.Insert(group.Key);

            editor.InsertParagraph();
            editor.ChangeParagraphListStyle(DefaultListStyles.Bulleted);
            foreach (var issue in group)
               editor.InsertLine(string.Format("* [{0}] {1}", issue.Key, issue.Summary));
         }

         var dialog = new RichTextPreview(document);
         dialog.ShowDialog();
      }

      private void ExportAsTextResults()
      {
         var resultBuilder = new StringBuilder();
         var grouped = Issues.OfType<JiraIssue>().GroupBy(i => i.EpicName);

         foreach(var group in grouped)
         {
            if (string.IsNullOrWhiteSpace(group.Key))
               resultBuilder.AppendLine("(No Epic)");
            else
               resultBuilder.AppendLine(group.Key);

            foreach(var issue in group)
               resultBuilder.AppendLine(string.Format("* [{0}] {1}", issue.Key, issue.Summary));
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
   }
}
