using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using JiraAssistant.Dialogs;
using JiraAssistant.Extensions;
using JiraAssistant.Model.Jira;
using JiraAssistant.Model.NavigationMessages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using Telerik.Windows.Data;
using Telerik.Windows.Persistence;

namespace JiraAssistant.ViewModel
{
   public class IssuesBrowserViewModel : ViewModelBase
   {
      private readonly string _settingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                                           "Yakuza", "Jira Assistant", "GridFilters");
      private readonly IMessenger _messenger;

      public RelayCommand SaveFiltersCommand { get; private set; }
      public BindableRadGridView Grid { get; set; }
      public RelayCommand LoadFiltersCommand { get; private set; }
      public RelayCommand ExportToConfluenceCommand { get; private set; }
      public RelayCommand PlainTextExportCommand { get; private set; }
      public RelayCommand OpenScrumCardsCommand { get; private set; }

      public QueryableCollectionView Issues { get; private set; }
      public JiraIssue SelectedIssue { get; set; }

      public IssuesBrowserViewModel(IList<JiraIssue> issues, IMessenger messenger)
      {
         Issues = new QueryableCollectionView(issues);
         _messenger = messenger;

         SaveFiltersCommand = new RelayCommand(SaveGridState);
         LoadFiltersCommand = new RelayCommand(LoadGridState);
         ExportToConfluenceCommand = new RelayCommand(ExportAsConfluenceMarkupResults);
         PlainTextExportCommand = new RelayCommand(ExportAsTextResults);
         OpenScrumCardsCommand = new RelayCommand(OpenScrumCards);
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
         var savedState = manager.Save(Grid);
         using (var reader = new StreamReader(savedState))
         using (var writer = new StreamWriter(Path.Combine(_settingsPath, name)))
         {
            writer.Write(reader.ReadToEnd());
         }
      }

      internal void PreviewSelectedIssue()
      {
         if (SelectedIssue != null)
            _messenger.Send(new OpenIssueDetailsMessage(SelectedIssue));
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
            manager.Load(Grid, stream);
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
         _messenger.Send(new OpenScrumCardsMessage(Issues.OfType<JiraIssue>().ToList()));
      }
   }
}
