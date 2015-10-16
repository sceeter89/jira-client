using System;
using System.Windows.Threading;
using LightShell.Messaging.Api;
using LightShell.Plugin.Jira.Api.Messages.Actions.Authentication;
using LightShell.Plugin.Jira.Api.Messages.IO.Jira;
using LightShell.Plugin.Jira.Api.Messages.Actions;
using LightShell.Api;

namespace LightShell.Service
{
   internal class ConnectionChecker : IMicroservice,
      IHandleMessage<LoggedInMessage>,
      IHandleMessage<LoggedOutMessage>,
      IHandleMessage<CheckJiraSessionResponse>
   {
      private IMessageBus _messageBus;
      private DispatcherTimer _timer;
      
      private void CheckTick(object sender, EventArgs e)
      {
         _messageBus.Send(new CheckJiraSessionMessage());
      }
      
      public void Handle(LoggedInMessage message)
      {
         _timer.IsEnabled = true;
      }

      public void Handle(LoggedOutMessage message)
      {
         _timer.IsEnabled = false;
      }

      public void Handle(CheckJiraSessionResponse message)
      {
         if (message.Response.IsLoggedIn == false)
         {
            _messageBus.Send(new ConnectionIsBroken());
            _timer.IsEnabled = false;
         }
      }

      public void Initialize(IMessageBus messageBus)
      {

         _messageBus = messageBus;
         _timer = new DispatcherTimer();
         _timer.Interval = TimeSpan.FromSeconds(3);
         _timer.Tick += CheckTick;

         messageBus.Register(this);
      }
   }
}
