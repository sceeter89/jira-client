using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using JiraManager.Controls;
using System.IO;
using System.Windows.Xps.Packaging;

namespace JiraManager.ViewModel
{
   public class MainViewModel : ViewModelBase
   {
      private readonly SearchIssuesViewModel _searchIssuesViewModel;

      public MainViewModel(SearchIssuesViewModel searchIssuesViewModel)
      {
         _searchIssuesViewModel = searchIssuesViewModel;
         SaveXpsCommand = new RelayCommand(SaveXps);
      }

      private void SaveXps()
      {
         var document = CardsPrintPreview.GenerateDocument(_searchIssuesViewModel.FoundIssues);
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

      public RelayCommand SaveXpsCommand { get; private set; }
   }
}