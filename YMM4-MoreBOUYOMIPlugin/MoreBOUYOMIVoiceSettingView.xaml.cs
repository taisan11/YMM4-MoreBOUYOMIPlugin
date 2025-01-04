using System.Windows.Controls;

namespace MoreBOUYOMIVoicePlugin {
    public partial class MoreBOUYOMIVoiceSettingView : UserControl {
        public MoreBOUYOMIVoiceSettingView()
        {
            InitializeComponent();
            DataContext = new MoreBOUYOMIVoiceSettingsViewModel();
        }
    }
}