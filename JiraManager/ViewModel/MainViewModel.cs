using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using JiraManager.Api;
using JiraManager.Controls;
using System.IO;
using System.Linq;
using System.Windows.Xps.Packaging;

namespace JiraManager.ViewModel
{
   public class MainViewModel : ViewModelBase
   {
      private readonly SearchIssuesViewModel _searchIssuesViewModel;
      private readonly IMessenger _messenger;

      public MainViewModel(IMessenger messenger, SearchIssuesViewModel searchIssuesViewModel)
      {
         _messenger = messenger;
         _searchIssuesViewModel = searchIssuesViewModel;
         SaveXpsCommand = new RelayCommand(SaveXps);
      }

      private void SaveXps()
      {
         if(_searchIssuesViewModel.FoundIssues.Any() == false)
         {
            _messenger.LogMessage("No issues to export.", LogLevel.Warning);
            return;
         }

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