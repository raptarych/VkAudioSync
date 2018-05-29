using System.Configuration;
using System.IO;
using Newtonsoft.Json;

namespace VkAudioSync
{
    public class SettingsManager
    {
        private static readonly string SettingsFilename = ConfigurationManager.AppSettings["settingsFilename"];
        private static SettingsModel _currentSettings;

        private static void CheckFile()
        {
            if (_currentSettings == null)
            {
                if (!File.Exists(SettingsFilename))
                {
                    _currentSettings = new SettingsModel();
                    File.WriteAllText(SettingsFilename, JsonConvert.SerializeObject(_currentSettings));
                }
                else
                {
                    _currentSettings = JsonConvert.DeserializeObject<SettingsModel>(File.ReadAllText(SettingsFilename));
                }
            }
            
        }

        private static void UpdateFile()
        {
            File.WriteAllText(SettingsFilename, JsonConvert.SerializeObject(_currentSettings));
        }

        public static string GetSid()
        {
            CheckFile();
            return _currentSettings.Sid;
        }
        public static string GetUid()
        {
            CheckFile();
            return _currentSettings.Uid;
        }
        public static void SetSid(string sid)
        {
            CheckFile();
            _currentSettings.Sid = sid;
            UpdateFile();
        }
        public static void SetUid(string uid)
        {
            CheckFile();
            _currentSettings.Uid = uid;
            UpdateFile();
        }
    }
}
