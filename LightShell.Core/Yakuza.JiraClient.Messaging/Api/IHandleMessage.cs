namespace Yakuza.JiraClient.Messaging.Api
{
   public interface IHandleMessage<T> where T: IMessage
   {
      void Handle(T message);
   }
}
