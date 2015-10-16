namespace LightShell.Messaging.Api
{
   public interface IHandleAllMessages
   {
      void Handle(IMessage message);
   }
}
