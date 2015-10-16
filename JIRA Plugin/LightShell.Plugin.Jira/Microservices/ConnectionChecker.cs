using System;
using LightShell.Api.Messages.Actions.Authentication;
using System.Windows.Threading;
using LightShell.Api.Messages.Actions;
using LightShell.Messaging.Api;
using LightShell.Api.Messages.IO.Jira;

namespace LightShell.Service
{
   internal class ConnectionChecker :
      IHandleMessage<LoggedInMessage>,
      IHandleMessage<LoggedOutMessage>,
      IHandleMessage<CheckJiraSessionResponse>
   {
      private readonly IMessageBus _messageBus;
      private readonly DispatcherTimer _timer;

      public ConnectionChecker(IMessageBus messageBus)
      {
         _messageBus = messageBus;
         _timer = new DispatcherTimer();
         _timer.Interval = TimeSpan.FromSeconds(3);
         _timer.Tick += CheckTick;

         messageBus.Register(this);
      }

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
   }
}
