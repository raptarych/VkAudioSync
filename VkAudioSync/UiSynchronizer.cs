using System;
using System.Windows;
using VkAudioSync.Views;

namespace VkAudioSync
{
    public static class UiSynchronizer
    {
        public static void Run(Action<InitWindow> act)
        {
            var appInstance = (App) Application.Current;
            appInstance.RunSyncWithUi(act);
        }

        public static void RunOnPage(Action<MusicLoaderPage> act)
        {
            var appInstance = (App)Application.Current;
            appInstance.RunSyncWithPageUi(act);
        }
    }
}
