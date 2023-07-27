using RaspiRemote.Models;
using Renci.SshNet;

namespace RaspiRemote.ViewModels.Gpio
{
    internal partial class GpioPinViewModel : BaseViewModel
    {
        private readonly GpioPinInfo _pinInfo;
        private readonly SshClient _sshClient;

        public GpioPinViewModel(GpioPinInfo pinInfo, SshClient sshClient)
        {
            _pinInfo = pinInfo;
            _sshClient = sshClient;
        }
    }
}
