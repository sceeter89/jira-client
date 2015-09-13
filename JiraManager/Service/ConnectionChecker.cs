using System;
using GalaSoft.MvvmLight.Messaging;
using JiraManager.Messages.Actions.Authentication;
using System.Windows.Threading;
using JiraManager.Api;
using JiraManager.Messages.Actions;

namespace JiraManager.Service
{
   class ConnectionChecker
   {
      private readonly IJiraOperations _operations;
      private readonly IMessenger _messenger;
      private readonly DispatcherTimer _timer;

      public ConnectionChecker(IMessenger messenger, IJiraOperations operations)
      {
         _operations = operations;
         _messenger = messenger;
         _messenger.Register<LoggedInMessage>(this, m => StartChecking());
         _messenger.Register<LoggedOutMessage>(this, m => StopChecking());
         _timer = new DispatcherTimer();
         _timer.Interval = TimeSpan.FromSeconds(3);
         _timer.Tick += CheckTick;
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

      private void StartChecking()
      {
         _timer.IsEnabled = true;
      }

      private void StopChecking()
      {
         _timer.IsEnabled = false;
      }
   }
}
