using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace VkAudioSync.Views
{
    /// <summary>
    ///     Логика взаимодействия для VkPage.xaml
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

        public string GetGetVkUid()
        {
            try
            {
                dynamic test = VkBrowser.Document;
                var content = test?.documentElement?.InnerHtml;
                if (content != null)
                    return new Regex("al_u[\\d]{1,20}").Match(content).ToString().Replace("al_u", "");
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Dictionary<string, string> GetTryGetCookie()
        {
            try
            {
                dynamic test = VkBrowser.Document;
                string rawCookie = test?.IHTMLDocument2_cookie;
                return rawCookie?.Split(';')
                    .Select(param => param.Trim())
                    .Select(i => i.Split('='))
                    .ToDictionary(i => i[0], i => i[1]);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}