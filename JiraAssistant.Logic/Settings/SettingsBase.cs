using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace JiraAssistant.Logic.Settings
{
    public class SettingsBase : INotifyPropertyChanged
    {
        private readonly string _filePath;
        private readonly string _settingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                                             "Yakuza", "Jira Assistant");
        private IDictionary<string, object> _settings = new Dictionary<string, object>();

        public event PropertyChangedEventHandler PropertyChanged;

        public SettingsBase()
        {
            var fileName = string.Format("{0}.json", GetType().Name);

            if (Directory.Exists(_settingsPath) == false)
                Directory.CreateDirectory(_settingsPath);

            _filePath = Path.Combine(_settingsPath, fileName);

            Load();
        }

        private void Load()
        {
            if (File.Exists(_filePath) == false)
                return;

            using (var reader = new StreamReader(_filePath))
            {
                _settings = JsonConvert.DeserializeObject<IDictionary<string, object>>(reader.ReadToEnd()) ?? _settings;
            }
        }

        private void Save()
        {
            using (var writer = new StreamWriter(_filePath))
            {
                _settings["$$__last_save_date__$$"] = DateTime.Now;
                writer.Write(JsonConvert.SerializeObject(_settings));
            }
        }

        protected DateTime GetValue(DateTime defaultValue, [CallerMemberName]string name = null)
        {
            if (_settings.ContainsKey(name) == false)
                return defaultValue;

            return DateTime.Parse(_settings[name].ToString());
        }

        protected TimeSpan GetValue(TimeSpan defaultValue, [CallerMemberName]string name = null)
        {
            if (_settings.ContainsKey(name) == false)
                return defaultValue;

            return TimeSpan.Parse(_settings[name].ToString());
        }

        protected T GetValue<T>(T defaultValue, [CallerMemberName]string name = null)
        {
            if (_settings.ContainsKey(name) == false)
                return defaultValue;

            return (T) Convert.ChangeType(_settings[name], typeof(T));
        }

        protected void SetValue<T>(T value, T defaultValue, [CallerMemberName]string name = null)
        {
            if (Equals(GetValue<T>(defaultValue, name: name), value))
                return;

            _settings[name] = value;
            RaisePropertyChanged(name);
            Save();
        }

        protected void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
