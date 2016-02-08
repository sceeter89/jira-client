using GalaSoft.MvvmLight.Command;
using JiraAssistant.Model.Jira;
using System.Collections.Generic;
using System.Windows.Input;
using System;
using JiraAssistant.Services;

namespace JiraAssistant.Pages
{
   public partial class SprintDetailsPage : BaseNavigationPage
   {
      private readonly IEnumerable<JiraIssue> _issues;
      private readonly INavigator _navigator;

      public SprintDetailsPage(RawAgileSprint sprint, IEnumerable<JiraIssue> issues, INavigator navigator)
      {
         InitializeComponent();

         Sprint = sprint;
         _issues = issues;
         _navigator = navigator;

         ScrumCardsCommand = new RelayCommand(OpenScrumCards);
         BurnDownCommand = new RelayCommand(OpenBurnDownChart, () => false);
         EngagementCommand = new RelayCommand(OpenEngagementChart, () => false);

         DataContext = this;
      }

      private void OpenEngagementChart()
      {
         throw new NotImplementedException();
      }

      private void OpenBurnDownChart()
      {
         throw new NotImplementedException();
      }

      private void OpenScrumCards()
      {
         _navigator.NavigateTo(new ScrumCardsPrintPreview(_issues));
      }

      public ICommand ScrumCardsCommand { get; private set; }
      public ICommand BurnDownCommand { get; private set; }
      public ICommand EngagementCommand { get; private set; }
      public RawAgileSprint Sprint { get; private set; }
   }
}
