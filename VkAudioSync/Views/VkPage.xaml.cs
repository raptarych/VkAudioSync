using System;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace VkAudioSync.Views
{
    /// <summary>
    /// Логика взаимодействия для VkPage.xaml
    /// </summary>
    public partial class VkPage : Page
    {
        public VkPage()
        {
            InitializeComponent();
            VkBrowser.Source = new Uri("https://m.vk.com");
        }

        public string GetContent
        {
            get
            {
                dynamic test = VkBrowser.Document;
                return test?.documentElement?.InnerHtml;
            }
        }
    }
}
