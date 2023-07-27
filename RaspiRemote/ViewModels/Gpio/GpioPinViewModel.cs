using RaspiRemote.Enums;
using RaspiRemote.Models;
using Renci.SshNet;

namespace RaspiRemote.ViewModels.Gpio
{
    internal partial class GpioPinViewModel : BaseViewModel
    {
        private readonly GpioPinInfo _pinInfo;
        private readonly SshClient _sshClient;

        public int PinNumber => (int)_pinInfo.Pin;

        public int State => _pinInfo.State ? 1 : 0;

        public GpioPinFunction Function
        {
            get => _pinInfo.Function;
            set
            {
                ChangePinFunction(value);
                OnPropertyChanged(nameof(Function));
            }
        }

        public GpioPinPull Pull
        {
            get => _pinInfo.Pull;
            set
            {
                ChangePinPull(value);
                OnPropertyChanged(nameof(Pull));
            }
        }

        public GpioPinViewModel(GpioPinInfo pinInfo, SshClient sshClient)
        {
            _pinInfo = pinInfo;
            _sshClient = sshClient;
        }

        private void ChangePinFunction(GpioPinFunction newValue) { }

        private void ChangePinPull(GpioPinPull newValue) { }
    }
}
