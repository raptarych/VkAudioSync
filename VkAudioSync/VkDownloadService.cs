using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;

namespace VkAudioSync
{
    public class VkDownloadService
    {
        private string Sid => SettingsManager.Get(SettingsRequisites.Sid);
        private string Uid => SettingsManager.Get(SettingsRequisites.Uid);
        private async Task DownloadBatch(List<VkSongModel> input)
        {
            input = input.Take(10).ToList();

            var client = new RestClient("https://vk.com");
            var request = new RestRequest("al_audio.php", Method.POST);
            request.AddParameter("act", "reload_audio");
            request.AddParameter("al", 1);
            request.AddParameter("ids", string.Join(",", input.Select(i => i.UniqueId)));

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
        }
    }
}
