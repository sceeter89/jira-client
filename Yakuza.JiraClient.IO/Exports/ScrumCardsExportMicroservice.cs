using System;
using System.IO;
using System.Windows.Xps.Packaging;
using Yakuza.JiraClient.Api;
using Yakuza.JiraClient.Api.Messages.IO.Exports;
using Yakuza.JiraClient.Controls.PrintPreview;
using Yakuza.JiraClient.Messaging.Api;

namespace Yakuza.JiraClient.IO.Exports
{
   public class ScrumCardsExportMicroservice : IMicroservice,
      IHandleMessage<GenerateScrumCardsMessage>
   {
      private readonly IMessageBus _messageBus;

      public ScrumCardsExportMicroservice(IMessageBus messageBus)
      {
         _messageBus = messageBus;

         _messageBus.Register(this);
      }

      public void Handle(GenerateScrumCardsMessage message)
      {
         var document = CardsPrintPreview.GenerateDocument(message.Issues);
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
   }
}
