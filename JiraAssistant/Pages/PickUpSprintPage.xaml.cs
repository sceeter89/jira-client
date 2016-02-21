using GalaSoft.MvvmLight.Command;
using JiraAssistant.Model.Jira;
using JiraAssistant.Model.Ui;
using JiraAssistant.Services;
using System;
using System.Collections.ObjectModel;

namespace JiraAssistant.Pages
{
   public partial class PickUpSprintPage : BaseNavigationPage
   {
      private readonly Func<RawAgileSprint, INavigationPage> _followUp;
      private readonly ObservableCollection<RawAgileSprint> _sprints;
      private readonly INavigator _navigator;

      public PickUpSprintPage(ObservableCollection<RawAgileSprint> sprints,
         Func<RawAgileSprint, INavigationPage> followUp,
         INavigator navigator)
      {
         InitializeComponent();

         Sprints = sprints;
         _followUp = followUp;
         _navigator = navigator;

         PickUpSprintCommand = new RelayCommand<RawAgileSprint>(PickUpSprint);

         DataContext = this;
      }

      private void PickUpSprint(RawAgileSprint sprint)
      {
         _navigator.NavigateTo(_followUp(sprint));
      }

      public RelayCommand<RawAgileSprint> PickUpSprintCommand { get; private set; }
      public ObservableCollection<RawAgileSprint> Sprints { get; set; }
   }
}
