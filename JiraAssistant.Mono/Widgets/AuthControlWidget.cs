using System;
using Autofac;
using JiraAssistant.Mono.Controllers;

namespace JiraAssistant.Mono
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class AuthControlWidget : Gtk.Bin
	{
		private readonly AuthControlController _controller;

		public AuthControlWidget()
		{
			this.Build();

			_controller = Bootstrap.IocContainer.Resolve<AuthControlController>(new NamedParameter("control", this));
		}

		public bool IsLoginControlVisible
		{
			get
			{
				return loginFrame.Visible;
			}
			set
			{
				loginFrame.Visible = value;
			}
		}

		public bool IsContentDisplayVisible
		{
			get
			{
				return contentDisplayWidget.Visible;
			}
			set
			{
				contentDisplayWidget.Visible = value;
			}
		}

		public string JiraAddress
		{
			get
			{
				return jiraAddress.Text;
			}
			set
			{
				jiraAddress.Text = value;
			}
		}

		public string Username
		{
			get
			{
				return username.Text;
			}
			set
			{
				username.Text = value;
			}
		}

		public ContentDisplayWidget ContentDisplay
		{
			get { return contentDisplayWidget; }
		}

		public event EventHandler<LoginAttemptEventArgs> LoginAttempt;

		protected void OnLoginClicked(object sender, EventArgs e)
		{
			if (LoginAttempt != null)
				LoginAttempt(this, new LoginAttemptEventArgs(
					jiraAddress.Text,
					username.Text,
					password.Text
				));
		}
	}
}
