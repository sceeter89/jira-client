using System.Windows.Media.Imaging;
using LightShell.Messaging.Api;

namespace LightShell.Plugin.Jira.Api.Messages.IO.Jira
{
   public class DownloadPictureMessage : IMessage
   {
      public DownloadPictureMessage(string pictureUrl)
      {
         PictureUrl = pictureUrl;
      }

      public string PictureUrl { get; private set; }
   }

   public class DownloadPictureResponse : IMessage
   {
      public DownloadPictureResponse(BitmapImage image)
      {
         Image = image;
      }

      public BitmapImage Image { get; private set; }
   }
}
