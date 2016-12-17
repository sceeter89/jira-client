﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using JiraAssistant.Domain.Exceptions;
using JiraAssistant.Domain.NavigationMessages;
using JiraAssistant.Logic.Settings;
using JiraAssistant.Domain.Jira;
using System;
using JiraAssistant.Logic.Extensions;

namespace JiraAssistant.Logic.ContextlessViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {
        private readonly JiraSessionViewModel _jiraSession;
        private readonly IMessenger _messenger;
        private string _loginErrorMessage;
        private string _jiraAddress;
        private string _username;
        private bool _isBusy;
        private string _busyMessage;
        private readonly IJiraApi _jiraApi;

        public LoginPageViewModel(IMessenger messenger,
           JiraSessionViewModel jiraSession,
           IJiraApi jiraApi,
           AssistantSettings configuration)
        {
            _messenger = messenger;
            _jiraSession = jiraSession;
            _jiraApi = jiraApi;

            JiraAddress = configuration.JiraUrl;
            Username = configuration.Username;
        }

        public RelayCommand<Func<string>> LoginCommand { get { return new RelayCommand<Func<string>>(Login); } }

        public string LoginErrorMessage
        {
            get { return _loginErrorMessage; }
            set
            {
                _loginErrorMessage = value;
                RaisePropertyChanged();
            }
        }

        public string JiraAddress
        {
            get { return _jiraAddress; }
            set
            {
                _jiraAddress = value;
                RaisePropertyChanged();
            }
        }

        public string Username
        {
            get { return _username; }
            set
            {
                _username = value;
                RaisePropertyChanged();
            }
        }

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                RaisePropertyChanged();
            }
        }

        public string BusyMessage
        {
            get { return _busyMessage; }
            set
            {
                _busyMessage = value;
                RaisePropertyChanged();
            }
        }

        private async void Login(Func<string> passwordGetter)
        {
            try
            {
                LoginErrorMessage = string.Empty;
                BusyMessage = "Trying to log into JIRA...";
                IsBusy = true;
                await _jiraApi.Session.AttemptLogin(JiraAddress, Username, passwordGetter());

                _messenger.Send(new OpenAgileBoardPickupMessage());
            }
            catch (ServerNotFoundException)
            {
                LoginErrorMessage = "Server did not respond. Please check address and/or connection.";
            }
            catch (LoginFailedException e)
            {
                LoginErrorMessage = e.Message;
            }
            catch (Exception e)
            {
                Sentry.CaptureException(e);
                LoginErrorMessage = "Failed to log in due to technical issues.";
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async void AttemptAutoLogin()
        {
            try
            {
                BusyMessage = "Checking existing credentials...";
                IsBusy = true;

                if (await _jiraApi.Session.CheckJiraSession())
                {
                    _messenger.Send(new OpenAgileBoardPickupMessage());
                    _jiraSession.LoggedIn();
                }
            }
            catch (ServerNotFoundException)
            {
                LoginErrorMessage = "Could not find most recent server. Try again later.";
            }
            catch (IncompleteJiraConfiguration) { }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
