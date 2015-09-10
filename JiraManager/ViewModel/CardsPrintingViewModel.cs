using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using JiraManager.Model;
using PdfSharp.Drawing;
using System.Collections.ObjectModel;
using System;
using System.Windows.Documents;
using JiraManager.Controls;
using System.IO;
using System.Windows.Xps.Packaging;
using System.Collections.Generic;
using System.Windows.Markup;
using System.Windows;

namespace JiraManager.ViewModel
{
   public class CardsPrintingViewModel : ViewModelBase
   {
      private readonly SearchIssuesViewModel _searchIssuesViewModel;
      private static readonly double A4Width = XUnit.FromCentimeter(21).Point;
      private static readonly double A4Height = XUnit.FromCentimeter(29.7).Point;

      public CardsPrintingViewModel(SearchIssuesViewModel searchIssuesViewModel)
      {
         _searchIssuesViewModel = searchIssuesViewModel;
         SaveXpsCommand = new RelayCommand(SaveXps);
      }

      private void SaveXps()
      {
         var document = GenerateDocument();
         var dlg = new Microsoft.Win32.SaveFileDialog();
         dlg.FileName = "Scrum Cards.xps";
         dlg.DefaultExt = ".xps";
         dlg.Filter = "XPS Documents (.xps)|*.xps";
         dlg.OverwritePrompt = true;

         var result = dlg.ShowDialog();

         if (result == false)
            return;

         var filename = dlg.FileName;
         if (File.Exists(filename))
            File.Delete(filename);

         using (var xpsd = new XpsDocument(filename, FileAccess.ReadWrite))
         {
            var xw = XpsDocument.CreateXpsDocumentWriter(xpsd);
            xw.Write(document);
            xpsd.Close();
         }
      }

      private FixedDocument GenerateDocument()
      {
         var document = new FixedDocument();
         var pageSize = new Size(8.5 * 96.0, 11.0 * 96.0);

         foreach (var pagePreview in CardsPrintPreview.GeneratePages(Issues))
         {
            var pageContent = new PageContent();
            var fixedPage = new FixedPage();
            pagePreview.Height = pageSize.Height - 10;
            pagePreview.Width = pageSize.Width - 10;
            pagePreview.Margin = new Thickness(5);
            pagePreview.UpdateLayout();

            fixedPage.Children.Add(pagePreview);
            ((IAddChild)pageContent).AddChild(fixedPage);
            document.Pages.Add(pageContent);
         }

         return document;
      }

      public ObservableCollection<JiraIssue> Issues
      {
         get
         {
            return _searchIssuesViewModel.FoundIssues;
         }
      }

      public RelayCommand SaveXpsCommand { get; private set; }
   }
}
