using System.IO;
using Newtonsoft.Json;

namespace VkAudioSync
{
    public class JsonFileManager : IFileManager
    {
        public void WriteFile<T>(string filePath, T data) where T : class
        {
            if (File.Exists(filePath)) File.Delete(filePath);
            using (var file = File.CreateText(filePath))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(file, data);
            }
            File.SetAttributes(filePath, FileAttributes.Hidden);
        }

        public T ReadFile<T>(string filePath) where T: class
        {
            if (!File.Exists(filePath)) return null;
            var fileData = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<T>(fileData);
        }
    }
}