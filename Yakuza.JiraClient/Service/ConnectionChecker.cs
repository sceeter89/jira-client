using System;
using Yakuza.JiraClient.Api.Messages.Actions.Authentication;
using System.Windows.Threading;
using Yakuza.JiraClient.Api;
using Yakuza.JiraClient.Api.Messages.Actions;
using Yakuza.JiraClient.Messaging.Api;

namespace Yakuza.JiraClient.Service
{
   internal class ConnectionChecker :
      IHandleMessage<LoggedInMessage>,
      IHandleMessage<LoggedOutMessage>
   {
      private readonly IJiraOperations _operations;
      private readonly IMessageBus _messenger;
      private readonly DispatcherTimer _timer;

      public ConnectionChecker(IMessageBus messenger, IJiraOperations operations)
      {
         _operations = operations;
         _messenger = messenger;
         _timer = new DispatcherTimer();
         _timer.Interval = TimeSpan.FromSeconds(3);
         _timer.Tick += CheckTick;

         messenger.Register(this);
      }

      private async void CheckTick(object sender, EventArgs e)
      {
         try
         {
            var result = await _operations.CheckSession();

            if (result.IsLoggedIn == false)
            {
               _messenger.Send(new ConnectionIsBroken());
               _timer.IsEnabled = false;
            }
         }
         catch { }
      }
      
      public void Handle(LoggedInMessage message)
      {
         _timer.IsEnabled = true;
      }

      public void Handle(LoggedOutMessage message)
      {
         _timer.IsEnabled = false;
      }
   }
}
