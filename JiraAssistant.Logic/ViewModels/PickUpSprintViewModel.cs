using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using JiraAssistant.Domain.Jira;
using JiraAssistant.Domain.NavigationMessages;
using JiraAssistant.Domain.Ui;
using System;
using System.Collections.Generic;

namespace JiraAssistant.Logic.ViewModels
{
    public class PickUpSprintViewModel
    {
        private readonly Func<RawAgileSprint, INavigationPage> _followUp;
        private readonly IMessenger _messenger;

        public PickUpSprintViewModel(IList<RawAgileSprint> sprints, Func<RawAgileSprint, INavigationPage> followUp, IMessenger messenger)
        {
            Sprints = sprints;
            _followUp = followUp;
            _messenger = messenger;

            PickUpSprintCommand = new RelayCommand<RawAgileSprint>(PickUpSprint);
        }

        private void PickUpSprint(RawAgileSprint sprint)
        {
            _messenger.Send(new OpenPageMessage(_followUp(sprint)));
        }

        public RelayCommand<RawAgileSprint> PickUpSprintCommand { get; private set; }
        public IList<RawAgileSprint> Sprints { get; set; }
    }
}
