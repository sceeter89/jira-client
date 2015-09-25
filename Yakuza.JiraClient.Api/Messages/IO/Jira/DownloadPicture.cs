using System.Windows.Media.Imaging;
using Yakuza.JiraClient.Messaging.Api;

namespace Yakuza.JiraClient.Api.Messages.IO.Jira
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
