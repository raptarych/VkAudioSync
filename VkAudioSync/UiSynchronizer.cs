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
            appInstance?.RunSyncWithUi(act);
        }

        public static void Run<T>(Action<InitWindow, T> act, T data)
        {
            var appInstance = (App)Application.Current;
            appInstance?.RunSyncWithUi(act, data);
        }
    }
}
