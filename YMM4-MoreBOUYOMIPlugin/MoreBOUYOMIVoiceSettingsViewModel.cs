using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MoreBOUYOMIVoicePlugin {
    public class MoreBOUYOMIVoiceSettingsViewModel : INotifyPropertyChanged {
        private string portNumber = "50080";
        private string wavPortNumber = "50088";

        public string PortNumber
        {
            get => portNumber;
            set {
                if (portNumber != value)
                {
                    portNumber = value;
                    OnPropertyChanged();
                }
            }
        }

        public string WavPortNumber
        {
            get => wavPortNumber;
            set {
                if (wavPortNumber != value)
                {
                    wavPortNumber = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
