using CommunityToolkit.Mvvm.ComponentModel;
using Renci.SshNet;

namespace RaspiRemote.ViewModels.Sensors
{
    public abstract partial class SensorViewModelBase : ObservableObject
    {
        private CancellationTokenSource _ctSource;
        private bool _ctSourceDisposed = false;

        protected readonly SshClient _sshClient;
        protected CancellationToken _ct;

        public SensorViewModelBase()
        {
            _sshClient = ServiceHelper.GetService<SshClientContainer>().SshClient;
        }

        public void StartUpdating()
        {
            StopUpdating();

            _ctSource = new CancellationTokenSource();
            _ct = _ctSource.Token;
            _ctSourceDisposed = false;

            Task.Run(Update, _ct);
        }

        public void StopUpdating()
        {
            if (_ctSource is not null && _ctSourceDisposed is false)
            {
                _ctSource.Cancel();
                _ctSource.Dispose();
                _ctSourceDisposed = true;
            }
        }

        protected abstract void Update();
    }
}
