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
            _settings = JsonConvert.DeserializeObject<IDictionary<string, object>>(reader.ReadToEnd());
         }
      }

      private void Save()
      {
         using (var stream = _storage.OpenFile(_filePath, FileMode.Create))
         using (var writer = new StreamWriter(stream))
         {
            SetValue(DateTime.Now, "$$__last_save_date__$$");
            writer.Write(JsonConvert.SerializeObject(_settings));
         }
      }

      protected T GetValue<T>(T defaultValue = default(T), [CallerMemberName]string name = null)
      {
         if (_settings.ContainsKey(name) == false)
            return defaultValue;

         return (T) _settings[name];
      }

      protected void SetValue<T>(T value, [CallerMemberName]string name = null)
      {
         if (GetValue<T>(name: name).Equals(value))
            return;

         _settings[name] = value;
         if (PropertyChanged != null)
            PropertyChanged(this, new PropertyChangedEventArgs(name));
         Save();
      }
   }
}
