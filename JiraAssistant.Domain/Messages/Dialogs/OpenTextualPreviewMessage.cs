namespace JiraAssistant.Domain.Messages.Dialogs
{
    public class OpenTextualPreviewMessage
    {
        public OpenTextualPreviewMessage(string content)
        {
            Content = content;
        }

        public string Content { get; private set; }
    }
}
