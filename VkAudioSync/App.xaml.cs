using System;
using System.Collections.Generic;
using System.Linq;
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

        public void RunSyncWithUi(Action<InitWindow> act)
        {
            mainWindow.synchronizationContext.Post(o =>
            {
                act(mainWindow);
            }, null);
        }

        public void RunSyncWithUi<T>(Action<InitWindow, T> act, T data)
        {
            mainWindow.synchronizationContext.Post(o =>
            {
                act(mainWindow, (T) o);
            }, data);
        }

        private async Task InitApp()
        {
            var sid = SettingsManager.Get(SettingsRequisites.Sid);
            var uid = SettingsManager.Get(SettingsRequisites.Uid);
            var dir = SettingsManager.Get(SettingsRequisites.Directory);

            if (string.IsNullOrEmpty(sid) || string.IsNullOrEmpty(uid))
            {
                RunSyncWithUi(o =>
                {
                    o.Content = new VkPage();
                });
                
                while (true)
                {
                    RunSyncWithUi(o =>
                    {
                        var vkPage = (VkPage) o.Content;
                        VkCookies = vkPage?.GetTryGetCookie();
                        Uid = vkPage?.GetGetVkUid();
                    });

                    var cookie = VkCookies;
                    if (cookie != null && cookie.ContainsKey("remixsid") && !string.IsNullOrEmpty(Uid) && Uid.All(i => i >= '0' && i <= '9'))
                    {
                        SettingsManager.Set(SettingsRequisites.Sid, cookie["remixsid"]);
                        SettingsManager.Set(SettingsRequisites.Uid, Uid);
                        dir = SettingsManager.Get(SettingsRequisites.Directory);

                        RunSyncWithUi(o =>
                        {
                            if (string.IsNullOrEmpty(dir))
                                o.Content = new DirExplorerPage();
                            else
                                o.Content = new MusicLoaderPage();
                        });
                        break;
                    }

                    await Task.Delay(500);
                }
            }
            else
            {
                RunSyncWithUi(o =>
                {
                    if (string.IsNullOrEmpty(dir))
                        mainWindow.Content = new DirExplorerPage();
                    else
                        mainWindow.Content = new MusicLoaderPage();
                });
            }
        }

        public void RunSyncWithPageUi(Action<MusicLoaderPage> act)
        {
            var page = (MusicLoaderPage) mainWindow.Content;
            if (page == null) return;
            mainWindow.synchronizationContext.Post(o =>
            {
                act(page);
            }, null);
        }
    }
}