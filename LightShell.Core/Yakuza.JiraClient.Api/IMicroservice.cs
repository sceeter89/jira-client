using LightShell.Messaging.Api;

namespace LightShell.Api
{
   public interface IMicroservice
   {
      void Initialize(IMessageBus messageBus);
   }
}
