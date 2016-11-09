using System;

namespace JiraAssistant.Domain.Messages
{
    public class ShowDesktopNotificationMessage
    {
        public ShowDesktopNotificationMessage(string title, string description, Action clickCallback, string iconResource)
        {
            Title = title;
            Description = description;
            ClickCallback = clickCallback;
            IconResource = iconResource;
        }

        public Action ClickCallback { get; private set; }
        public string Description { get; private set; }
        public string IconResource { get; private set; }
        public string Title { get; private set; }
    }
}
