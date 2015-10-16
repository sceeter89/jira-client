using System;
using LightShell.Messaging.Api;

namespace LightShell.InternalMessages.UI
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
