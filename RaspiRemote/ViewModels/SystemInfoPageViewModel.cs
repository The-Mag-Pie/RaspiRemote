using Renci.SshNet;

namespace RaspiRemote.ViewModels
{
    internal partial class SystemInfoPageViewModel : BaseViewModel
    {
        private readonly SshClient _sshClient;

        public SystemInfoPageViewModel(SshClientContainer sshClientContainer)
        {
            _sshClient = sshClientContainer.SshClient;
        }
    }
}
