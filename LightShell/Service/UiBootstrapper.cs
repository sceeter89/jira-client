using System;
using System.Collections.Generic;
using System.Linq;
using LightShell.InternalMessages.UI;
using LightShell.Messaging.Api;
using LightShell.ViewModel;

namespace LightShell.Service
{
   internal class UiBootstrapper :
      IHandleMessage<ViewModelInitializedMessage>,
      IHandleMessage<PluginsLoadedMessage>
   {
      private readonly IMessageBus _messageBus;
      private readonly ISet<Type> _requiredViewModels = new HashSet<Type>
      {
         typeof(MainViewModel),
         typeof(MenuBarViewModel),
         typeof(LogViewModel),
      };

      public UiBootstrapper(IMessageBus messageBus)
      {
         _messageBus = messageBus;

         _messageBus.Register(this);
      }

      public void Handle(PluginsLoadedMessage message)
      {
         _messageBus.Send(new UpdateUiBootstrapMessage("Application ready!"));
         _messageBus.Send(new ApplicationLoadedMessage());
      }

      public void Handle(ViewModelInitializedMessage message)
      {
         if (_requiredViewModels.Contains(message.ViewModelType) == false)
            return;

         _requiredViewModels.Remove(message.ViewModelType);

         if (_requiredViewModels.Any() == false)
         {
            _messageBus.Send(new CoreUserInterfaceLoadedMessage());
            _messageBus.Send(new UpdateUiBootstrapMessage("Loading plugins..."));
         }
      }
   }
}
