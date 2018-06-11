using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;

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

            var dir = SettingsManager.Get(SettingsRequisites.Directory);
            TbUid.Text = SettingsManager.Get(SettingsRequisites.DownloadForUid);
            if (string.IsNullOrEmpty(TbUid.Text))
            {
                TbUid.Text = SettingsManager.Get(SettingsRequisites.Uid);
                SettingsManager.Set(SettingsRequisites.DownloadForUid, TbUid.Text);
            }
            if (string.IsNullOrEmpty(dir))
            {
                BtRefresh.IsEnabled = false;
                return;
            }

            LbDirectory.Content = dir;
            var pathName = Path.Combine(SettingsManager.Get(SettingsRequisites.Directory), ".playlist");
            var idsDownload = new JsonFileManager().ReadFile<List<VkSongModel>>(pathName);
            LbIdDownloader.Content = $"{idsDownload?.Count ?? 0} файлов";

            SyncLabels();
        }

        public void SyncLabels()
        {
            var pathName = Path.Combine(SettingsManager.Get(SettingsRequisites.Directory), ".playlist");
            var idsDownload = new JsonFileManager().ReadFile<List<VkSongModel>>(pathName);
            if (idsDownload == null) return; 
            var sync = new SongsFileSynchronizer();
            LbIndexed.Content = $"{sync.GetSongsIntersect(idsDownload)} файлов";

            var countToDownload = sync.SongsToDownloadCount(idsDownload);
            LbToDownload.Content = $"+ {countToDownload} файлов";
            if (countToDownload > 0) LbToDownload.Foreground = new SolidColorBrush(Colors.Green);

            var countToDelete = sync.SongsToDeleteCount(idsDownload);
            LbToDelete.Content = $"- {countToDelete} файлов";
            if (countToDelete > 0) LbToDelete.Foreground = new SolidColorBrush(Colors.Red);
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var sid = SettingsManager.Get(SettingsRequisites.Sid);
            var uid = SettingsManager.Get(SettingsRequisites.DownloadForUid);
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
                    TbUid.Text = SettingsManager.Get(SettingsRequisites.DownloadForUid);
                    SyncLabels();
                });
            });

        }

        public bool MarkerToStopDownload { get; private set; }

        private void StartDownloadOnClick(object sender, System.Windows.RoutedEventArgs e)
        {
            MarkerToStopDownload = false;
            Task.Run(async () =>
            {
                var vkService = new VkDownloadService();
                var pathName = Path.Combine(SettingsManager.Get(SettingsRequisites.Directory), ".playlist");
                var audioPlaylist = new JsonFileManager().ReadFile<List<VkSongModel>>(pathName);

                var audioToDownload = new SongsFileSynchronizer().GetSongsToDownload(audioPlaylist);

                await vkService.DeleteAndDownload(audioToDownload, new SongsFileSynchronizer().GetSongsToDelete(audioPlaylist));
            });
        }

        private void StopDownloadOnClick(object sender, System.Windows.RoutedEventArgs e)
        {
            MarkerToStopDownload = true;
        }

        private void BtDirectory_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.SelectedPath = SettingsManager.Get(SettingsRequisites.Directory);
                var result = dialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    LbDirectory.Content = dialog.SelectedPath;
                    BtRefresh.IsEnabled = true;
                    SettingsManager.Set(SettingsRequisites.Directory, dialog.SelectedPath);
                }
            }
        }

        private void TbUid_TextChanged(object sender, TextChangedEventArgs e)
        {
            SettingsManager.Set(SettingsRequisites.DownloadForUid, TbUid.Text);
        }
    }
}
