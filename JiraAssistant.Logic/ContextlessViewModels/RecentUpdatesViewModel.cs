using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.ObjectModel;
using JiraAssistant.Domain.Ui;
using JiraAssistant.Domain.Messages;
using System.Linq;

namespace JiraAssistant.Logic.ContextlessViewModels
{
    public class RecentUpdatesViewModel : ViewModelBase
    {
        public RecentUpdatesViewModel(IMessenger messenger)
        {
            messenger.Register<IssueUpdatedMessage>(this, NewUpdate);
            Updates = new ObservableCollection<IssueUpdateDetails>();
        }

        private void NewUpdate(IssueUpdatedMessage message)
        {
            var updateDetails = new IssueUpdateDetails
            {
                Issue = message.Issue,
                ChangeTime = message.Occurred,
                ChangeSummary = message.Changes.Select(c => new FieldChange
                {
                    FieldName = c.Field,
                    OriginalValue = c.FromString,
                    NewValue = c.toString
                })
            };

            Updates.Insert(0, updateDetails);
        }

        public ObservableCollection<IssueUpdateDetails> Updates { get; private set; }
    }
}
