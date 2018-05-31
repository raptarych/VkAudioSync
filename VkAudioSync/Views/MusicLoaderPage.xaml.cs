using System.Collections.Generic;
using System.IO;
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
            IdDownloaderLabel.Content = $"{idsDownload?.Count ?? 0} файлов";
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var sid = SettingsManager.Get(SettingsRequisites.Sid);
            var uid = SettingsManager.Get(SettingsRequisites.Uid);
            IdDownloaderLabel.Content = "Загрузка...";
            var audioPlaylist = VkAudioHelper.GetUsersPlaylist(uid, sid);

            var pathName = Path.Combine(SettingsManager.Get(SettingsRequisites.Directory), ".playlist");
            new JsonFileManager().WriteFile(pathName, audioPlaylist);
            IdDownloaderLabel.Content = $"{audioPlaylist.Count} файлов";
        }
    }
}
