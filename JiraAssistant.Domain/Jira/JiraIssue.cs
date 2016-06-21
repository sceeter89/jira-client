using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;

namespace JiraAssistant.Domain.Jira
{
    public class JiraIssue : IEquatable<JiraIssue>
    {
        public static readonly Version ModelVersion = new Version(1, 1, 0, 0);

        public string Assignee { get; set; }
        public DateTime Created { get; set; }
        public string Description { get; set; }
        public string Key { get; set; }
        public string Priority { get; set; }
        public string Project { get; set; }
        public string Reporter { get; set; }
        public DateTime? Resolved { get; set; }
        public string Status { get; set; }
        public float StoryPoints { get; set; }
        public int Subtasks { get; set; }
        public string Summary { get; set; }
        public string EpicLink { get; set; }
        public string EpicName { get; set; }
        public RawFields BuiltInFields { get; set; }
        public RawChangesHistory[] Changelog { get; set; }

        public string Labels
        {
            get
            {
                return string.Join(", ", BuiltInFields.Labels);
            }
            set { }
        }

        public IEnumerable<int> SprintIds { get; set; }

        public override bool Equals(object other)
        {
            return Equals(other as JiraIssue);
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }

        public bool Equals(JiraIssue other)
        {
            if ((object) other == null)
            {
                return false;
            }
            return Key == other.Key;
        }

        public static bool operator ==(JiraIssue left, JiraIssue right)
        {
            return object.Equals(left, right);
        }

        public static bool operator !=(JiraIssue left, JiraIssue right)
        {
            return !(left == right);
        }
    }

    public class JiraIssuePrintPreviewModel : INotifyPropertyChanged
    {
        private Color _categoryColor;

        public JiraIssuePrintPreviewModel()
        {
            CategoryColor = Colors.White;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public int Row { get; set; }
        public int Column { get; set; }
        public JiraIssue Issue { get; set; }
        public Color CategoryColor
        {
            get { return _categoryColor; }
            set
            {
                _categoryColor = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("CategoryColor"));
            }
        }
    }
}
