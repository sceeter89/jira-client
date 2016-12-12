using GalaSoft.MvvmLight.Messaging;
using Gtk;
using JiraAssistant.Domain.Ui;
using JiraAssistant.Domain.Messages;
using JiraAssistant.Domain;

namespace JiraAssistant.Mono
{
	public class DesktopNotificationsService
	{
		private readonly IMessenger _messenger;
		private readonly IInvokeOnUiThread _onUiThread;


		public DesktopNotificationsService(IMessenger messenger,
		                                   IInvokeOnUiThread onUiThread)
		{
			_messenger = messenger;
			_onUiThread = onUiThread;

			_messenger.Register<ShowDesktopNotificationMessage>(this, ShowDesktopNotification);
			_messenger.Register<ShowAlertMessage>(this, ShowAlert);
		}

		void ShowAlert(ShowAlertMessage message)
		{
			_onUiThread.Invoke(() =>
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
