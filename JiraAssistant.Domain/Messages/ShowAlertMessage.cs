using System;

namespace JiraAssistant.Domain.Messages
{
    public class ShowAlertMessage
    {
        public ShowAlertMessage(string message)
        {
            Message = message;
        }
        
        public string Message { get; private set; }
    }
}
