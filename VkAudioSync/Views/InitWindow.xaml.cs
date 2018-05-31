using System;
using System.Threading;
using System.Windows;

namespace VkAudioSync.Views
{
    /// <summary>
    /// Логика взаимодействия для InitPage.xaml
    /// </summary>
    public partial class InitWindow : Window
    {
        public readonly SynchronizationContext synchronizationContext;

        public void RunSync(Action<InitWindow> act)
        {
            synchronizationContext.Post(o =>
            {
                act(this);
            }, null);
        }
        public InitWindow()
        {
            InitializeComponent();
            synchronizationContext = SynchronizationContext.Current;
        }
    }
}
