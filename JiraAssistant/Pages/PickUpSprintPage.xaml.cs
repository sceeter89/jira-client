using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using JiraAssistant.Model.Jira;
using JiraAssistant.Model.NavigationMessages;
using JiraAssistant.Model.Ui;
using System;
using System.Collections.Generic;

namespace JiraAssistant.Pages
{
   public partial class PickUpSprintPage : BaseNavigationPage
   {
      private readonly Func<RawAgileSprint, INavigationPage> _followUp;
      private readonly IMessenger _messenger;

      public PickUpSprintPage(IList<RawAgileSprint> sprints,
         Func<RawAgileSprint, INavigationPage> followUp,
         IMessenger messenger)
      {
         InitializeComponent();

         Sprints = sprints;
         _followUp = followUp;
         _messenger = messenger;

         PickUpSprintCommand = new RelayCommand<RawAgileSprint>(PickUpSprint);

         DataContext = this;
      }

      private void PickUpSprint(RawAgileSprint sprint)
      {
         _messenger.Send(new OpenPageMessage(_followUp(sprint)));
      }

      public RelayCommand<RawAgileSprint> PickUpSprintCommand { get; private set; }
      public IList<RawAgileSprint> Sprints { get; set; }
      public override string Title { get { return "Sprints selection"; } }
   }
}
