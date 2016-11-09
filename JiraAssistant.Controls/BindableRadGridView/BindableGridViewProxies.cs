using System.Collections.Generic;
using System.ComponentModel;
using Telerik.Windows.Data;

namespace JiraAssistant.Controls.BindableRadGridView
{
#pragma warning disable JustCode_CSharp_TypeFileNameMismatch // Types not matching file names
   public class SortDescriptorProxy
   {
      public string ColumnUniqueName { get; set; }
      public ListSortDirection SortDirection { get; set; }
   }

   public class GroupDescriptorProxy
   {
      public string ColumnUniqueName { get; set; }
      public ListSortDirection? SortDirection { get; set; }
   }

   public class FilterDescriptorProxy
   {
      public FilterOperator Operator { get; set; }
      public object Value { get; set; }
      public bool IsCaseSensitive { get; set; }
   }

   public class FilterSetting
   {
      public string ColumnUniqueName { get; set; }

      private List<object> _selectedDistinctValue;
      public List<object> SelectedDistinctValues
      {
         get
         {
            if (this._selectedDistinctValue == null)
            {
               this._selectedDistinctValue = new List<object>();
            }

            return this._selectedDistinctValue;
         }
      }

      public FilterDescriptorProxy Filter1 { get; set; }
      public FilterCompositionLogicalOperator FieldFilterLogicalOperator { get; set; }
      public FilterDescriptorProxy Filter2 { get; set; }
   }

#pragma warning restore JustCode_CSharp_TypeFileNameMismatch // Types not matching file names
}
