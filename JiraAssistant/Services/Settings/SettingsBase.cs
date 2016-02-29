using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.CompilerServices;

namespace JiraAssistant.Services.Settings
{
   public class SettingsBase : INotifyPropertyChanged
   {
      private readonly string _filePath;
      private readonly IsolatedStorageFile _storage = IsolatedStorageFile.GetUserStoreForAssembly();
      private IDictionary<string, object> _settings = new Dictionary<string, object>();

      public event PropertyChangedEventHandler PropertyChanged;

      public SettingsBase()
      {
         var fileName = string.Format("{0}.json", GetType().Name);

         if (_storage.DirectoryExists("Settings") == false)
            _storage.CreateDirectory("Settings");

         _filePath = Path.Combine("Settings", fileName);

         Load();
      }

      private void Load()
      {
         if (_storage.FileExists(_filePath) == false)
            return;

         using (var stream = _storage.OpenFile(_filePath, FileMode.Open))
         using (var reader = new StreamReader(stream))
         {
            _settings = JsonConvert.DeserializeObject<IDictionary<string, object>>(reader.ReadToEnd()) ?? _settings;
         }
      }

      private void Save()
      {
         using (var stream = _storage.OpenFile(_filePath, FileMode.Create))
         using (var writer = new StreamWriter(stream))
         {
            _settings["$$__last_save_date__$$"] = DateTime.Now;
            writer.Write(JsonConvert.SerializeObject(_settings));
         }
      }

      protected T GetValue<T>(T defaultValue, [CallerMemberName]string name = null)
      {
         if (_settings.ContainsKey(name) == false)
            return defaultValue;

         return (T) _settings[name];
      }

      protected void SetValue<T>(T value, T defaultValue, [CallerMemberName]string name = null)
      {
         if (Equals(GetValue<T>(defaultValue, name: name), value))
            return;

         _settings[name] = value;
         if (PropertyChanged != null)
            PropertyChanged(this, new PropertyChangedEventArgs(name));
         Save();
      }
   }
}
