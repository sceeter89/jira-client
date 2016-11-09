using JiraAssistant.Domain.Ui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JiraAssistant.Domain.Messages
{
    public class PerformApplicationUpdateMessage
    {
        public PerformApplicationUpdateMessage(UpdateMethod method)
        {
            Method = method;
        }

        public UpdateMethod Method { get; private set; }
    }
}
