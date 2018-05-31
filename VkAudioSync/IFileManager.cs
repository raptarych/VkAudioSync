namespace VkAudioSync
{
    public interface IFileManager
    {
        T ReadFile<T>(string filePath) where T : class;
        void WriteFile<T>(string filePath, T data) where T : class;
    }
}