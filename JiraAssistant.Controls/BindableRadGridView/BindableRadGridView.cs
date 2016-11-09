using JiraAssistant.Controls.Dialogs;
using JiraAssistant.Logic.Extensions;
using NLog;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using Telerik.Windows.Controls;
using Telerik.Windows.Persistence;
using Telerik.Windows.Persistence.Services;
using JiraAssistant.Domain.Jira;
using System.Collections.Generic;
using Telerik.Windows.Data;

namespace JiraAssistant.Controls.BindableRadGridView
{
    public class BindableRadGridView : RadGridView, IGridView
    {
        private readonly string _settingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                                             "Yakuza", "Jira Assistant", "GridFilters");
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public static readonly DependencyProperty ColumnsCollectionProperty =
              DependencyProperty.RegisterAttached("ColumnsCollection", typeof(ObservableCollection<GridViewDataColumn>),
                                                  typeof(BindableRadGridView),
                                                  new PropertyMetadata(OnColumnsCollectionChanged));



        public IEnumerable<JiraIssue> Issues
        {
            get { return (IEnumerable<JiraIssue>) GetValue(IssuesProperty); }
            set { SetValue(IssuesProperty, value); }
        }

        public static readonly DependencyProperty IssuesProperty =
            DependencyProperty.Register("Issues", typeof(IEnumerable<JiraIssue>), typeof(BindableRadGridView), new PropertyMetadata(OnIssuesChanged));

        private static void OnIssuesChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var gridView = obj as BindableRadGridView;
            if (gridView == null) return;

            gridView.ItemsSource = new QueryableCollectionView(e.NewValue as IEnumerable<JiraIssue>);
        }

        public BindableRadGridView() : base()
        {
            ServiceProvider.RegisterPersistenceProvider<ICustomPropertyProvider>(typeof(BindableRadGridView), new BindableGridViewPropertyProvider());
        }

        private static void OnColumnsCollectionChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var gridView = o as BindableRadGridView;
            if (gridView == null) return;

            gridView.Columns.Clear();
            if (e.NewValue == null)
                return;

            var collection = e.NewValue as ObservableCollection<GridViewDataColumn>;

            if (collection == null) return;

            foreach (var column in collection)
            {
                gridView.Columns.Add(new GridViewDataColumn
                {
                    Header = column.Header,
                    DataMemberBinding = column.DataMemberBinding,
                    IsReadOnly = column.IsReadOnly
                });
            }
        }

        public ObservableCollection<GridViewDataColumn> ColumnsCollection
        {
            get { return (ObservableCollection<GridViewDataColumn>) GetValue(ColumnsCollectionProperty); }
            set { SetValue(ColumnsCollectionProperty, value); }
        }

        public void SaveGridStateTo()
        {
            try
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
                var savedState = manager.Save(this);
                using (var reader = new StreamReader(savedState))
                using (var writer = new StreamWriter(Path.Combine(_settingsPath, name)))
                {
                    writer.Write(reader.ReadToEnd());
                }
            }
            catch (Exception e)
            {
                Sentry.CaptureException(e);
                MessageBox.Show("Failed to save filter!", "Jira Assistant", MessageBoxButton.OK, MessageBoxImage.Error);
                _logger.Error(e, "Error while saving Issue Browser filter!");
            }
        }

        public void LoadGridStateFrom()
        {
            string filterPath = null;
            try
            {
                var filters = Directory.EnumerateFiles(_settingsPath).Select(p => Path.GetFileName(p)).ToArray();
                var dialog = new SelectFilterDialog(filters);
                if (dialog.ShowDialog() == false)
                    return;
                filterPath = Path.Combine(_settingsPath, dialog.FilterName);

                var manager = new PersistenceManager();
                manager.AllowCrossVersion = true;
                using (var stream = File.OpenRead(filterPath))
                {
                    manager.Load(this as RadGridView, stream);
                }
            }
            catch (Exception e)
            {
                Sentry.CaptureException(e);
                MessageBox.Show("Failed to load filter!", "Jira Assistant", MessageBoxButton.OK, MessageBoxImage.Error);
                _logger.Error(e, "Error while loading Issue Browser filter from: " + filterPath);
            }
        }

        public IEnumerable<JiraIssue> GetFilteredIssues()
        {
            if (ItemsSource is QueryableCollectionView == false)
                return Enumerable.Empty<JiraIssue>();

            var model = ItemsSource as QueryableCollectionView;

            return model.OfType<JiraIssue>();
        }
    }
}