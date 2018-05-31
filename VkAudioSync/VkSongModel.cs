using Newtonsoft.Json;
using VkAudioSync.Vk;

namespace VkAudioSync
{
    public class VkSongModel
    {
        public static VkSongModel FromJson(object[] model)
        {
            return new VkSongModel
            {
                SongId = model[0].ToString(),
                OwnerId = model[1].ToString(),
                Url = model[2].ToString(),
                VkTitle = model[3].ToString(),
                VkArtist = model[4].ToString()
            };
        }
        public string SongId { get; set; }
        public string OwnerId { get; set; }
        [JsonIgnore]
        public string UniqueId => $"{OwnerId}_{SongId}";
        public string VkTitle { get; set; }
        public string VkArtist { get; set; }
        public string Url { get; set; }
        [JsonIgnore]
        public bool Unmasked => !Url.Contains("audio_api_unavailable");
        [JsonIgnore]
        public string FileName => $"{VkArtist} - {VkTitle}.mp3";

        public void Unmask(string uid)
        {
            Url = VkAudioUnmasker.UnmaskFrom(Url, uid);
        }
    }
}
