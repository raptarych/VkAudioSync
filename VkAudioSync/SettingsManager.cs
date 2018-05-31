using System.Collections.Generic;
using System.Configuration;
using System.IO;
using Newtonsoft.Json;

namespace VkAudioSync
{
    public static class SettingsManager
    {
        private static readonly string SettingsFilename = ConfigurationManager.AppSettings["settingsFilename"];
        private static Dictionary<SettingsRequisites, string> _currentSettings;

        private static void CheckFile()
        {
            if (_currentSettings == null)
            {
                if (!File.Exists(SettingsFilename))
                {
                    _currentSettings = new Dictionary<SettingsRequisites, string>();
                    File.WriteAllText(SettingsFilename, JsonConvert.SerializeObject(_currentSettings));
                }
                else
                {
                    _currentSettings = JsonConvert.DeserializeObject<Dictionary<SettingsRequisites, string>>(File.ReadAllText(SettingsFilename));
                }
            }
            
        }

        private static void UpdateFile()
        {
            File.WriteAllText(SettingsFilename, JsonConvert.SerializeObject(_currentSettings));
        }

        public static string Get(SettingsRequisites key)
        {
            CheckFile();
            return _currentSettings.TryGetValue(key, out var value) ? value : null;
        }

        public static void Set(SettingsRequisites key, string value)
        {
            CheckFile();
            _currentSettings[key] = value;
            UpdateFile();
        }
    }
}
