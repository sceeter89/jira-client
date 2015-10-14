using System;
using System.Collections.Generic;
using System.Linq;
using Yakuza.JiraClient.Messaging.Api;

namespace Yakuza.JiraClient.Messaging
{
   public class MessageBus : IMessageBus
   {
      private readonly IDictionary<Type, IList<object>> _typeHandlers = new Dictionary<Type, IList<object>>();
      private readonly IList<IHandleAllMessages> _allMessagesHandlers = new List<IHandleAllMessages>();
      private readonly IDictionary<Type, IList<dynamic>> _concreteListeners = new Dictionary<Type, IList<dynamic>>();

      public void Listen<TMessage>(Action<TMessage> handler) where TMessage : IMessage
      {
         var key = typeof(TMessage);
         if (_concreteListeners.ContainsKey(key) == false)
            _concreteListeners[key] = new List<dynamic>();

         _concreteListeners[key].Add(handler);
      }

      public void Register<TListener>(TListener listener)
      {
         var type = listener.GetType();

         foreach (var handlerType in type.GetInterfaces()
               .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IHandleMessage<>)))
         {
            var messageType = handlerType.GenericTypeArguments[0];
            if (_typeHandlers.ContainsKey(messageType) == false)
               _typeHandlers[messageType] = new List<dynamic>();

            _typeHandlers[messageType].Add(listener);
         }
      }

      public void Send<TMessage>(TMessage message) where TMessage : IMessage
      {
         var messageType = typeof(TMessage);

         if (_concreteListeners.ContainsKey(messageType))
         {
            foreach (var action in _concreteListeners[messageType].ToList())
               ((Action<TMessage>)action)(message);
         }

         if (_typeHandlers.ContainsKey(messageType))
         {
            foreach (var handler in _typeHandlers[messageType].ToList())
               (handler as IHandleMessage<TMessage>).Handle(message);
         }

         foreach (var handler in _allMessagesHandlers.ToList())
            handler.Handle(message);
      }

      public void SubscribeAllMessages(IHandleAllMessages handler)
      {
         _allMessagesHandlers.Add(handler);
      }

      public void SendWithReply<TMessage, TReply>(TMessage message)
         where TMessage : IDirectMessage<TReply>
         where TReply : IMessage
      {
         Send(message);
      }
   }
}
