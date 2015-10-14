using System;

namespace Yakuza.JiraClient.Messaging.Api
{
   public interface IMessageBus
   {
      void Register<TListener>(TListener listener);
      void Listen<TMessage>(Action<TMessage> handler) where TMessage : IMessage;
      void Send<TMessage>(TMessage message) where TMessage : IMessage;
      void SendWithReply<TMessage, TReply>(TMessage message) where TMessage : IDirectMessage<TReply>
                                                             where TReply : IMessage;
      void SubscribeAllMessages(IHandleAllMessages handler);
   }
}
