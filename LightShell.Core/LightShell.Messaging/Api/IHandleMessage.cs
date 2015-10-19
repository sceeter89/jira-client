namespace LightShell.Messaging.Api
{
   public interface IHandleMessage<T> where T: IMessage
   {
      void Handle(T message);
   }
}
