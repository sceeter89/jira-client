using System;
using System.Collections.Generic;
using System.Linq;
using Yakuza.JiraClient.Messaging.Api;

namespace Yakuza.JiraClient.Messaging
{
   public class MessageBus : IMessageBus
   {
      private readonly IDictionary<Type, IList<dynamic>> _typeHandlers = new Dictionary<Type, IList<dynamic>>();
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
         var type = typeof(TListener);
         foreach (var handlerType in type.GetInterfaces().Where(i => i == typeof(IHandleMessage<>)))
         {
            if (_concreteListeners.ContainsKey(handlerType) == false)
               _concreteListeners[handlerType] = new List<dynamic>();

            _concreteListeners[handlerType].Add(listener);
         }
      }

      public void Send<TMessage>(TMessage message) where TMessage : IMessage
      {
         var messageType = typeof(TMessage);

         if (_concreteListeners.ContainsKey(messageType))
         {
            foreach(var action in _concreteListeners[messageType])
               action(message);
         }

         if(_typeHandlers.ContainsKey(messageType))
         {
            foreach(var handler in _typeHandlers[messageType])
               handler.Handle(message);
         }

         foreach(var handler in _allMessagesHandlers)
            handler.Handle(message);
      }

      public void SubscribeAllMessages(IHandleAllMessages handler)
      {
         _allMessagesHandlers.Add(handler);
      }
   }
}
