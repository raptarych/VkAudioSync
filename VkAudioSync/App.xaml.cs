using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using VkAudioSync.Views;

namespace VkAudioSync
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        private InitWindow mainWindow;
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
                mainWindow.synchronizationContext.Post(o =>
                {
                    mainWindow.Content = new VkPage();
                }, null);

                await Task.Delay(3000);
                while (true)
                {
                    var cookie = new CookieGetter().GetVkCookie();
                    if (cookie.ContainsKey("remixsid"))
                    {
                        SettingsManager.SetSid(cookie["remixsid"]);

                        mainWindow.synchronizationContext.Post(o =>
                        {
                            var vkPage = (VkPage)mainWindow.Content;
                            var content = vkPage.GetContent;
                            /*if (string.IsNullOrEmpty(content))
                            {
                                Thread.Sleep(1000);
                                continue;
                            }*/
                            uid = new Regex("al_u[\\d]{1,20}").Match(content).ToString().Replace("al_u", "");
                            if (!string.IsNullOrEmpty(uid) && uid.All(i => i >= '0' && i <= '9'))
                            {
                                mainWindow.Content = new MusicLoaderPage();
                            }
                            
                        }, null);
                        break;
                    }
                    await Task.Delay(1000);
                }
            }
            else
            {
                mainWindow.synchronizationContext.Post(o =>
                {
                    mainWindow.Content = new MusicLoaderPage();
                }, null);
            }
        }
    }
}
