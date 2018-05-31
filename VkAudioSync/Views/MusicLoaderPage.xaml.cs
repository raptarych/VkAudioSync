using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace VkAudioSync.Views
{
    /// <summary>
    /// Логика взаимодействия для VkPage.xaml
    /// </summary>
    public partial class MusicLoaderPage : Page
    {
        public MusicLoaderPage()
        {
            InitializeComponent();
            var pathName = Path.Combine(SettingsManager.Get(SettingsRequisites.Directory), ".playlist");
            var idsDownload = new JsonFileManager().ReadFile<List<VkSongModel>>(pathName);
            LbIdDownloader.Content = $"{idsDownload?.Count ?? 0} файлов";

            SyncLabels(idsDownload);
        }

        private void SyncLabels(List<VkSongModel> idsDownload)
        {
            if (idsDownload == null) return;
            var sync = new SongsFileSynchronizer();
            LbIndexed.Content = $"{sync.GetSongsIntersect(idsDownload)} файлов";
            LbToDownload.Content = $"{sync.SongsToDownloadCount(idsDownload)} файлов";
            LbToDelete.Content = $"{sync.GetSongsToDelete(idsDownload)} файлов";
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var sid = SettingsManager.Get(SettingsRequisites.Sid);
            var uid = SettingsManager.Get(SettingsRequisites.Uid);
            LbIdDownloader.Content = "Загрузка...";
            Task.Run(async () =>
            {
                var vkService = new VkDownloadService();
                var audioPlaylist = await vkService.GetUsersPlaylist(uid, sid);

                var pathName = Path.Combine(SettingsManager.Get(SettingsRequisites.Directory), ".playlist");
                new JsonFileManager().WriteFile(pathName, audioPlaylist);

                UiSynchronizer.Run(window =>
                {
                    LbIdDownloader.Content = $"{audioPlaylist.Count} файлов";
                    SyncLabels(audioPlaylist);
                });
            });

        }

        private void Button_Click_1(object sender, System.Windows.RoutedEventArgs e)
        {
            Task.Run(async () =>
            {
                var vkService = new VkDownloadService();
                var pathName = Path.Combine(SettingsManager.Get(SettingsRequisites.Directory), ".playlist");
                var audioPlaylist = new JsonFileManager().ReadFile<List<VkSongModel>>(pathName);

                audioPlaylist = new SongsFileSynchronizer().GetSongsToDownload(audioPlaylist);

                await vkService.DownloadSongs(audioPlaylist);
            });
        }
    }
}
