using LightShell.Messaging.Api;
using LightShell.Plugin.Jira.Api.Messages.Actions.Authentication;
using LightShell.Plugin.Jira.Api.Messages.Status;
using System;
using System.Windows.Input;

namespace LightShell.Plugin.Jira.Api
{
   public class LoginEnabledRelayCommand : ICommand,
      IHandleMessage<LoggedInMessage>,
      IHandleMessage<LoggedOutMessage>,
      IHandleMessage<ConnectionDownMessage>,
      IHandleMessage<ConnectionEstablishedMessage>
   {
      private readonly IMessageBus _messageBus;
      private bool _isConnected;
      private readonly Action _onClick;

      public event EventHandler CanExecuteChanged;

      public LoginEnabledRelayCommand(Action onClick, IMessageBus messageBus)
      {
         _onClick = onClick;
         _messageBus = messageBus;
         _messageBus.Register(this);
      }

      public bool CanExecute(object parameter)
      {
         return _isConnected;
      }

      public void Execute(object parameter)
      {
         _onClick();
      }

      public void Handle(LoggedOutMessage message)
      {
         IsConnected = false;
      }

      public void Handle(LoggedInMessage message)
      {
         IsConnected = true;
      }

      public void Handle(ConnectionDownMessage message)
      {
         IsConnected = false;
      }

      public void Handle(ConnectionEstablishedMessage message)
      {
         IsConnected = true;
      }

      private bool IsConnected
      {
         get {return _isConnected;}
         set
         {
            if(_isConnected == value)
               return;
            _isConnected = value;
            RaiseCanExecuteChanged();
         }
      }

      private void RaiseCanExecuteChanged()
      {
         if (CanExecuteChanged != null)
            CanExecuteChanged(this, EventArgs.Empty);
      }
   }
}
