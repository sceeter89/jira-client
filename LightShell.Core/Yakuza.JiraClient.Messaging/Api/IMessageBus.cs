using System;

namespace LightShell.Messaging.Api
{
   public interface IMessageBus
   {
      void Register<TListener>(TListener listener);
      void Listen<TMessage>(Action<TMessage> handler) where TMessage : IMessage;
      void Send<TMessage>(TMessage message) where TMessage : IMessage;
      void SubscribeAllMessages(IHandleAllMessages handler);
   }
}
