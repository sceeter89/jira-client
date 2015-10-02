using Yakuza.JiraClient.Messaging.Api;

namespace Yakuza.JiraClient.Api
{
   public interface IMicroservice
   {
      void Initialize(IMessageBus messageBus);
   }
}
