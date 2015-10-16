using System;
using Yakuza.JiraClient.Messaging.Api;

namespace Yakuza.JiraClient.InternalMessages.UI
{
   internal class ViewModelInitializedMessage : IMessage
   {
      public ViewModelInitializedMessage(Type viewModelType)
      {
         ViewModelType = viewModelType;
      }

      public Type ViewModelType { get; private set; }
   }
}
