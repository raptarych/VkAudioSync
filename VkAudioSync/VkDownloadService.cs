using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;

namespace VkAudioSync
{
    public class VkDownloadService
    {
        private int BatchSize => 10;
        private string Sid => SettingsManager.Get(SettingsRequisites.Sid);
        private string Uid => SettingsManager.Get(SettingsRequisites.Uid);
        private string Directory => SettingsManager.Get(SettingsRequisites.Directory);
        private async Task DownloadBatch(IEnumerable<VkSongModel> batch)
        {
            batch = batch.Take(BatchSize).ToList();

            var client = new RestClient("https://vk.com");
            var request = new RestRequest("al_audio.php", Method.POST);
            request.AddParameter("act", "reload_audio");
            request.AddParameter("al", 1);
            request.AddParameter("ids", string.Join(",", batch.Select(i => i.UniqueId)));

            request.AddHeader("origin", "https://vk.com");
            request.AddHeader("referer", $"https://vk.com/audios{Uid}");
            request.AddHeader("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/66.0.3359.139 Safari/537.36");

            request.AddCookie("remixsid", Sid);

            var response = client.Execute(request);
            var content = response.Content;
            var jsonContent = new Regex("\\[\\[.+\\]\\]").Match(content).Value;
            var data = JsonConvert.DeserializeObject<List<object[]>>(jsonContent)
                .Select(VkSongModel.FromJson)
                .ToList();
            data.ForEach(x => x.Unmask(Uid));

            foreach (var vkSongModel in data)
            {
                var webClient = new WebClient();
                var filePath = Path.Combine(Directory, vkSongModel.FileName);
                webClient.DownloadProgressChanged += (sender, args) =>
                {
                    UiSynchronizer.RunOnPage(o =>
                    {
                        o.LbProgress.Content = $"Скачивание {vkSongModel.FileName}: {args.TotalBytesToReceive / args.BytesReceived * 100}%";
                    });
                };
                await webClient.DownloadFileTaskAsync(vkSongModel.Url, filePath);
            }
        }

        public async Task DownloadSongs(List<VkSongModel> songsData)
        {
            for (var i = 0; i <= songsData.Count / BatchSize; i++)
            {
                await DownloadBatch(songsData.Skip(i * BatchSize).Take(BatchSize));
            }
        }

        public async Task<List<VkSongModel>> GetUsersPlaylist(string uid, string sid)
        {
            var firstBatch = (await GetUsersPlaylistWithOffset(uid, sid, 0)).Take(30).ToList();
            Thread.Sleep(2000);
            firstBatch.AddRange(await GetUsersPlaylistWithOffset(uid, sid, 30));
            return firstBatch;
        }

        private async Task<List<VkSongModel>> GetUsersPlaylistWithOffset(string uid, string sid, int offset)
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

            var response = await client.ExecuteTaskAsync(request);
            var encoding = Encoding.GetEncoding("Windows-1251");
            var content = encoding.GetString(response.RawBytes);
            var jsonContent = new Regex("\\[\\[.+\\]\\]").Match(content).Value;
            var data = JsonConvert.DeserializeObject<List<object[]>>(jsonContent)
                .Select(VkSongModel.FromJson)
                .ToList();
            return data;
        }
    }
}
