using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;

namespace MoreBOUYOMIVoicePlugin {
    public partial class MoreBOUYOMIVoiceSettingView : UserControl {
        public MoreBOUYOMIVoiceSettingView()
        {
            InitializeComponent();
        }

        private void BrowseExecutable_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not MoreBOUYOMIVoiceSettings settings)
                return;

            var dialog = new OpenFileDialog
            {
                Title = "棒読みちゃんの実行ファイルを選択",
                Filter = "実行ファイル (*.exe)|*.exe",
                CheckFileExists = true,
                Multiselect = false,
            };
            if (dialog.ShowDialog() == true)
                settings.BouyomiExecutablePath = dialog.FileName;
        }
    }
}