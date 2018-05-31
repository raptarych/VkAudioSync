using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VkAudioSync
{
    public class SongsFileSynchronizer
    {

        private List<string> GetMp3FileList()
        {
            var dir = SettingsManager.Get(SettingsRequisites.Directory);
            return Directory.GetFiles(dir, "*.mp3").Select(i => i.Split('\\').Last()).ToList();
        } 
        public int GetSongsIntersect(List<VkSongModel> vkSongs)
        {
            var hash = new HashSet<string>(vkSongs.Select(i => i.FileName));
            hash.IntersectWith(GetMp3FileList());
            return hash.Count;
        }

        public int SongsToDownloadCount(List<VkSongModel> vkSongs)
        {
            return GetSongsToDownload(vkSongs).Count;
        }

        public List<VkSongModel> GetSongsToDownload(List<VkSongModel> vkSongs)
        {
            var hash = new HashSet<string>(vkSongs.Select(i => i.FileName));
            hash.ExceptWith(GetMp3FileList());
            return vkSongs.Where(i => hash.Contains(i.FileName)).ToList();
        }

        public int GetSongsToDelete(List<VkSongModel> vkSongs)
        {
            var hash = new HashSet<string>(GetMp3FileList());
            hash.ExceptWith(vkSongs.Select(i => i.FileName));
            return hash.Count;
        }
    }
}
