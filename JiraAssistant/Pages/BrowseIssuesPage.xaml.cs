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
using Telerik.Windows.Persistence.Services;
using JiraAssistant.Extensions;
using Telerik.Windows.Persistence;
using System.IO;
using System.Windows;

namespace JiraAssistant.Pages
{
   public partial class BrowseIssuesPage
   {
      private readonly INavigator _navigator;
      private readonly IComponentContext _iocContainer;
      private readonly string _settingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                                           "Yakuza", "Jira Assistant", "GridFilters");

      public BrowseIssuesPage(IList<JiraIssue> issues, IComponentContext iocContainer)
      {
         InitializeComponent();
         ServiceProvider.RegisterPersistenceProvider<ICustomPropertyProvider>(typeof(BindableRadGridView), new BindableGridViewPropertyProvider());
         if (Directory.Exists(_settingsPath) == false)
            Directory.CreateDirectory(_settingsPath);

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
            Tooltip = "Export as Confluence Markup",
            Command = new RelayCommand(ExportAsConfluenceMarkupResults),
            Icon = new BitmapImage(new Uri(@"pack://application:,,,/;component/Assets/Icons/ConfluenceIcon.png"))
         });
         Buttons.Add(new ToolbarButton
         {
            Tooltip = "Save current filter",
            Command = new RelayCommand(SaveGridState),
            Icon = new BitmapImage(new Uri(@"pack://application:,,,/;component/Assets/Icons/SaveIcon.png"))
         });
         Buttons.Add(new ToolbarButton
         {
            Tooltip = "Load saved filter",
            Command = new RelayCommand(LoadGridState),
            Icon = new BitmapImage(new Uri(@"pack://application:,,,/;component/Assets/Icons/FilterIcon.png"))
         });

         DataContext = this;
      }

      private void SaveGridState()
      {
         var dialog = new FilterNameDialog();
         if (dialog.ShowDialog() == false)
            return;

         var name = Regex.Replace(dialog.FilterName, @"[^\w\s]", "_");

         if (File.Exists(Path.Combine(_settingsPath, name)))
         {
            var result = MessageBox.Show("Do you want to overwrite existing filter?", "Jira Assistant", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.No)
               return;
         }

         var manager = new PersistenceManager();
         manager.AllowCrossVersion = true;
         var savedState = manager.Save(grid);
         using (var reader = new StreamReader(savedState))
         using (var writer = new StreamWriter(Path.Combine(_settingsPath, name)))
         {
            writer.Write(reader.ReadToEnd());
         }
      }

      private void LoadGridState()
      {
         var filters = Directory.EnumerateFiles(_settingsPath).Select(p => Path.GetFileName(p)).ToArray();
         var dialog = new SelectFilterDialog(filters);
         if (dialog.ShowDialog() == false)
            return;

         var manager = new PersistenceManager();
         manager.AllowCrossVersion = true;
         using (var stream = File.OpenRead(Path.Combine(_settingsPath, dialog.FilterName)))
         {
            manager.Load(grid, stream);
         }
      }

      private void ExportAsConfluenceMarkupResults()
      {
         var resultBuilder = new StringBuilder();
         var grouped = Issues.OfType<JiraIssue>().GroupBy(i => i.EpicName);

         foreach (var group in grouped)
         {
            resultBuilder.AppendLine();
            if (string.IsNullOrWhiteSpace(group.Key))
               resultBuilder.AppendLine("h2. (No Epic)");
            else
               resultBuilder.AppendLine("h2. " + group.Key);
            resultBuilder.AppendLine();

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
