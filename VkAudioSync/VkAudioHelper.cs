using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Newtonsoft.Json;
using RestSharp;

namespace VkAudioSync
{
    internal static class VkAudioHelper
    {
        public static List<VkSongModel> GetUsersPlaylist(string uid, string sid)
        {
            var firstBatch = GetUsersPlaylistWithOffset(uid, sid, 0).Take(30).ToList();
            Thread.Sleep(2000);
            firstBatch.AddRange(GetUsersPlaylistWithOffset(uid, sid, 30));
            return firstBatch;
        }

        private static List<VkSongModel> GetUsersPlaylistWithOffset(string uid, string sid, int offset)
        {
            var client = new RestClient("https://vk.com");
            var request = new RestRequest("al_audio.php", Method.POST);
            request.AddParameter("act", "load_section");
            request.AddParameter("al", 1);
            request.AddParameter("owner_id", uid);
            request.AddParameter("type", "playlist");
            request.AddParameter("playlist_id", -1);
            if (offset > 0) request.AddParameter("offset", offset);

            request.AddHeader("origin", "https://vk.com");
            request.AddHeader("referer", $"https://vk.com/audios{uid}");
            request.AddHeader("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/66.0.3359.139 Safari/537.36");

            request.AddCookie("remixsid", sid);

            var response = client.Execute(request);
            var content = response.Content;
            var jsonContent = new Regex("\\[\\[.+\\]\\]").Match(content).Value;
            var data = JsonConvert.DeserializeObject<List<object[]>>(jsonContent)
                .Select(VkSongModel.FromJson)
                .ToList();
            return data;
        }

        public static List<List<string>> GetSongsData(string[] ids, string uid, string sid)
        {
            var client = new RestClient("https://vk.com");
            var request = new RestRequest("al_audio.php", Method.POST);
            request.AddParameter("act", "reload_audio");
            request.AddParameter("al", 1);
            request.AddParameter("ids", string.Join(",", ids));

            request.AddHeader("origin", "https://vk.com");
            request.AddHeader("referer", $"https://vk.com/audios{uid}");
            request.AddHeader("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/66.0.3359.139 Safari/537.36");

            request.AddCookie("remixsid", sid);

            var response = client.Execute(request);
            var content = response.Content;
            var jsonContent = new Regex("\\[\\[.+\\]\\]").Match(content).Value;
            var data = JsonConvert.DeserializeObject<List<List<string>>>(jsonContent);
            return data;
        }
    }
}
