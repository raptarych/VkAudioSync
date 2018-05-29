using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using VkAudioSync.Views;

namespace VkAudioSync
{
    /// <summary>
    ///     Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        private InitWindow mainWindow;

        private Dictionary<string, string> VkCookies { get; set; } = new Dictionary<string, string>();
        private string Uid { get; set; }

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            mainWindow = new InitWindow();
            Task.Run(async () =>
            {
                await Task.Delay(1000);
                await InitApp();
            });


            mainWindow.Show();
        }

        private async Task InitApp()
        {
            var sid = SettingsManager.GetSid();
            var uid = SettingsManager.GetUid();

            if (string.IsNullOrEmpty(sid) || string.IsNullOrEmpty(uid))
            {
                mainWindow.synchronizationContext.Post(o => { mainWindow.Content = new VkPage(); }, null);
                
                while (true)
                {
                    mainWindow.synchronizationContext.Post(o =>
                    {
                        var vkPage = (VkPage) mainWindow.Content;
                        VkCookies = vkPage?.GetTryGetCookie();
                        Uid = vkPage?.GetGetVkUid();
                    }, null);

                    var cookie = VkCookies;
                    if (cookie != null && cookie.ContainsKey("remixsid") && !string.IsNullOrEmpty(Uid) && Uid.All(i => i >= '0' && i <= '9'))
                    {
                        SettingsManager.SetSid(cookie["remixsid"]);
                        SettingsManager.SetUid(Uid);

                        mainWindow.synchronizationContext.Post(o =>
                        {
                            mainWindow.Content = new MusicLoaderPage();
                        }, null);
                        break;
                    }

                    await Task.Delay(500);
                }
            }
            else
            {
                mainWindow.synchronizationContext.Post(o => { mainWindow.Content = new MusicLoaderPage(); }, null);
            }
        }
    }
}