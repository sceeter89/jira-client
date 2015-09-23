namespace Yakuza.JiraClient.Messaging.Api
{
   public interface IHandleAllMessages
   {
      void Handle(IMessage message);
   }
}
