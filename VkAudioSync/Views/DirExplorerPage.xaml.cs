using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace VkAudioSync.Views
{
    /// <summary>
    /// Логика взаимодействия для DirExplorerPage.xaml
    /// </summary>
    public partial class DirExplorerPage : Page
    {
        private readonly object dummyNode = null;

        public DirExplorerPage()
        {
            InitializeComponent();
        }

        private void DirectoryExplorerTreeView_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var s in Directory.GetLogicalDrives())
            {
                var item = new TreeViewItem
                {
                    Header = s,
                    Tag = s,
                    FontWeight = FontWeights.Normal
                };
                item.Items.Add(dummyNode);
                item.Expanded += folder_Expanded;
                DirectoryExplorerTreeView.Items.Add(item);
            }
        }

        void folder_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)sender;
            if (item.Items.Count == 1 && item.Items[0] == dummyNode)
            {
                item.Items.Clear();
                try
                {
                    foreach (var s in Directory.GetDirectories(item.Tag.ToString()))
                    {
                        var subitem = new TreeViewItem
                        {
                            Header = s.Substring(s.LastIndexOf("\\", StringComparison.Ordinal) + 1),
                            Tag = s,
                            FontWeight = FontWeights.Normal
                        };
                        subitem.Items.Add(dummyNode);
                        subitem.Expanded += folder_Expanded;
                        item.Items.Add(subitem);
                    }
                }
                catch (Exception) { }
            }
        }

        private void DirectoryExplorerTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var item = (TreeViewItem) e.NewValue;
            PathLabel.Content = item.Tag;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var path = (string) PathLabel.Content;
            SettingsManager.Set(SettingsRequisites.Directory, path);
            ((InitWindow) Parent).Content = new MusicLoaderPage();
        }
    }
}
