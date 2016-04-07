using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using Telerik.Windows.Controls;

namespace JiraAssistant.Extensions
{
   public class BindableRadGridView : RadGridView
   {
      public static readonly DependencyProperty ColumnsCollectionProperty =
            DependencyProperty.RegisterAttached("ColumnsCollection", typeof(ObservableCollection<GridViewDataColumn>),
                                                typeof(BindableRadGridView),
                                                new PropertyMetadata(OnColumnsCollectionChanged));

      public BindableRadGridView()
      {
         this.Unloaded += (sender, args) => Columns.Clear();
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
   }
}