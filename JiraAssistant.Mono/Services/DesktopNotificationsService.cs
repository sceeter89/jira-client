using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using Gtk;
using JiraAssistant.Domain.Messages;

namespace JiraAssistant.Mono
{
	public class DesktopNotificationsService
	{
		private readonly IMessenger _messenger;

		public DesktopNotificationsService(IMessenger messenger)
		{
			_messenger = messenger;

			_messenger.Register<ShowDesktopNotificationMessage>(this, ShowDesktopNotification);
			_messenger.Register<ShowAlertMessage>(this, ShowAlert);
		}

		void ShowAlert(ShowAlertMessage message)
		{
			DispatcherHelper.CheckBeginInvokeOnUI(() =>
			{
				var dialog = new MessageDialog(null, DialogFlags.Modal, MessageType.Other, ButtonsType.Ok, message.Message);
				dialog.Run();
				dialog.Destroy();
			});
		}

		void ShowDesktopNotification(ShowDesktopNotificationMessage message)
		{
			//TODO Implement cross-platform desktop notifications
		}
	}
}
