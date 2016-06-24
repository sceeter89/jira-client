namespace JiraAssistant.Domain.Ui
{
    public class FieldChange
    {
        public string FieldName { get; set; }
        public string OriginalValue { get; set; }
        public string NewValue { get; set; }
    }
}