namespace VkAudioSync
{
    public interface IFileManager
    {
        T ReadFile<T>(string filePath);
        void WriteFile<T>(string filePath, T data);
    }
}