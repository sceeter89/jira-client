using System;
using GalaSoft.MvvmLight.Messaging;
using JiraAssistant.Domain.Messages;
using Telerik.Windows.Controls;
using System.Windows.Controls;
using System.Windows.Media;
using GalaSoft.MvvmLight.Command;
using System.Windows;
using GalaSoft.MvvmLight.Threading;

namespace JiraAssistant
{
    public class DesktopNotificationsService
    {
        private readonly RadDesktopAlertManager _alertManager;
        private readonly IMessenger _messenger;
        private readonly ImageSourceConverter _imageSourceConverter = new ImageSourceConverter();

        public DesktopNotificationsService(IMessenger messenger)
        {
            _messenger = messenger;
            _alertManager = new RadDesktopAlertManager();
            _messenger.Register<ShowDesktopNotificationMessage>(this, ShowDesktopNotification);
            _messenger.Register<ShowAlertMessage>(this, ShowAlert);
        }

        private void ShowAlert(ShowAlertMessage message)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            MessageBox.Show(message.Message, "JIRA Assistant", MessageBoxButton.OK, MessageBoxImage.Information));
        }

        private void ShowDesktopNotification(ShowDesktopNotificationMessage message)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                var icon = message.IconResource != null ?
                    new Image { Source = GetImageFromResources(message.IconResource), Width = 48, Height = 48 } :
                    null;
                _alertManager.ShowAlert(new RadDesktopAlert
                {
                    Header = message.Title,
                    Content = message.Description,
                    ShowDuration = 5000,
                    Icon = icon,
                    IconColumnWidth = 48,
                    Command = message.ClickCallback != null ? new RelayCommand(message.ClickCallback) : null
                });
            });
        }

        protected ImageSource GetImageFromResources(string path)
        {
            var uri = string.Format("pack://application:,,,/JiraAssistant;component/{0}", path.TrimStart('/'));
            return _imageSourceConverter.ConvertFromString(uri) as ImageSource;
        }
    }
}
