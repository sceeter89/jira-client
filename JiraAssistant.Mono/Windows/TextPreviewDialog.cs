using System;
using System.IO;
using Gtk;

namespace JiraAssistant.Mono.Windows
{
	public partial class TextPreviewDialog : Dialog
	{
		private readonly string _suggestedName;
		private readonly string _content;

		public TextPreviewDialog(string content, string suggestedName = null)
		{
			this.Build();

			_content = content;
			_suggestedName = suggestedName;

			textView.Buffer = new TextBuffer(null) { Text = _content };

			saveAsButton.Clicked += SaveButtonClicked;
		}

		private void SaveButtonClicked(object sender, EventArgs e)
		{
			var fileChooser = new FileChooserDialog("Where to save content", this, FileChooserAction.Save,
													"Cancel", ResponseType.Cancel,
													"Open", ResponseType.Accept);
			fileChooser.Filter = new FileFilter();
			fileChooser.Filter.AddPattern("*.*");
			fileChooser.Filter.AddPattern("*.csv");
			fileChooser.Filter.AddPattern("*.txt");

			if (_suggestedName != null)
				fileChooser.CurrentName = _suggestedName;

			if (fileChooser.Run() == (int)ResponseType.Accept)
			{
				File.WriteAllText(fileChooser.Filename, _content);
			}

			fileChooser.Destroy();
		}
	}
}
