using System.IO;
using Newtonsoft.Json;

namespace VkAudioSync
{
    public class JsonFileManager : IFileManager
    {
        public void WriteFile<T>(string filePath, T data)
        {
            using (var file = File.CreateText(filePath))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(file, data);
            }
            File.SetAttributes(filePath, FileAttributes.Hidden);
        }

        public T ReadFile<T>(string filePath)
        {
            using (var file = File.OpenText(filePath))
            {
                var serializer = new JsonSerializer();
                return (T) serializer.Deserialize(file, typeof(T));
            }
        }
    }
}