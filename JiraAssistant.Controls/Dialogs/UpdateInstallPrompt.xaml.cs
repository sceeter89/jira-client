using GalaSoft.MvvmLight.Command;
using System;
using System.Windows;

namespace JiraAssistant.Controls.Dialogs
{
   public partial class UpdateInstallPrompt
   {
      public UpdateInstallPrompt(Version current, Version latest, bool isStable)
      {
         InitializeComponent();

         CurrentVersion = current;
         LatestVersion = latest;
         StableVersionNoticeVisibility = isStable ? Visibility.Visible : Visibility.Collapsed;
         PrereleaseVersionNoticeVisibility = isStable ? Visibility.Collapsed : Visibility.Visible;

         InstallOnExitCommand = new RelayCommand(() =>
         {
            Result = UpdatePromptResult.InstallOnExit;
            Close();
         });
         CloseAndInstallCommand = new RelayCommand(() =>
         {
            Result = UpdatePromptResult.ExitAndInstall;
            Close();
         });
         ManualInstallCommand = new RelayCommand(() =>
         {
            Result = UpdatePromptResult.InstallManually;
            Close();
         });
         CancelCommand = new RelayCommand(() =>
         {
            Result = UpdatePromptResult.None;
            Close();
         });

         Result = UpdatePromptResult.None;

         DataContext = this;
      }

      public Version CurrentVersion { get; private set; }
      public Version LatestVersion { get; private set; }

      public Visibility StableVersionNoticeVisibility { get; private set; }
      public Visibility PrereleaseVersionNoticeVisibility { get; private set; }

      public UpdatePromptResult Result { get; private set; }
      public RelayCommand InstallOnExitCommand { get; private set; }
      public RelayCommand CloseAndInstallCommand { get; private set; }
      public RelayCommand ManualInstallCommand { get; private set; }
      public RelayCommand CancelCommand { get; private set; }

      public UpdatePromptResult Prompt()
      {
         ShowDialog();

         return Result;
      }
   }

   public enum UpdatePromptResult
   {
      InstallOnExit, ExitAndInstall, InstallManually, None
   }
}
