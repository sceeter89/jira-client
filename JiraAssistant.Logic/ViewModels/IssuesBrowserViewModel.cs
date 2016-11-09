using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using JiraAssistant.Domain.Jira;
using JiraAssistant.Domain.Messages.Dialogs;
using JiraAssistant.Domain.NavigationMessages;
using JiraAssistant.Logic.Extensions;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace JiraAssistant.Logic.ViewModels
{
    public class IssuesBrowserViewModel : ViewModelBase
    {
        private readonly IMessenger _messenger;

        public RelayCommand SaveFiltersCommand { get; private set; }
        public IGridView Grid { get; set; }
        public RelayCommand LoadFiltersCommand { get; private set; }
        public RelayCommand ExportToConfluenceCommand { get; private set; }
        public RelayCommand PlainTextExportCommand { get; private set; }
        public RelayCommand OpenScrumCardsCommand { get; private set; }
        
        public JiraIssue SelectedIssue { get; set; }
        public IList<JiraIssue> Issues { get; private set; }

        public IssuesBrowserViewModel(IList<JiraIssue> issues, IMessenger messenger)
        {
            Issues = issues;
            _messenger = messenger;
            
            SaveFiltersCommand = new RelayCommand(SaveGridState);
            LoadFiltersCommand = new RelayCommand(LoadGridState);
            ExportToConfluenceCommand = new RelayCommand(ExportAsConfluenceMarkupResults);
            PlainTextExportCommand = new RelayCommand(ExportAsTextResults);
            OpenScrumCardsCommand = new RelayCommand(OpenScrumCards);
        }

        private void SaveGridState()
        {
            Grid.SaveGridStateTo();
        }

        public void PreviewSelectedIssue()
        {
            if (SelectedIssue != null)
                _messenger.Send(new OpenIssueDetailsMessage(SelectedIssue));
        }

        private void LoadGridState()
        {
            Grid.LoadGridStateFrom();
        }

        private void ExportAsConfluenceMarkupResults()
        {
            var resultBuilder = new StringBuilder();
            var grouped = Grid.GetFilteredIssues().GroupBy(i => i.EpicName);

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

            _messenger.Send(new OpenTextualPreviewMessage(resultBuilder.ToString()));
        }

        private string EscapeConfluenceMarkupCharacters(string summary)
        {
            return Regex.Replace(summary, @"[{\[\]\}\(\)!@\\]", m => "\\" + m.Value);
        }

        private void ExportAsTextResults()
        {
            var resultBuilder = new StringBuilder();
            var grouped = Grid.GetFilteredIssues().GroupBy(i => i.EpicName);

            foreach (var group in grouped)
            {
                if (string.IsNullOrWhiteSpace(group.Key))
                    resultBuilder.AppendLine("\n(No Epic)");
                else
                    resultBuilder.AppendLine("\n" + group.Key);

                foreach (var issue in group)
                    resultBuilder.AppendLine(string.Format("* {0} - {1}", issue.Key, issue.Summary));
            }

            _messenger.Send(new OpenTextualPreviewMessage(resultBuilder.ToString()));
        }

        private void OpenScrumCards()
        {
            _messenger.Send(new OpenScrumCardsMessage(Grid.GetFilteredIssues().ToList()));
        }
    }
}
