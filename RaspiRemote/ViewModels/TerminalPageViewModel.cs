using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Renci.SshNet;

namespace RaspiRemote.ViewModels
{
    internal partial class TerminalPageViewModel : BaseViewModel
    {
        private readonly SshClient _sshClient;
        private readonly ShellStream _shellStream;

        public event Action<string> ConsoleDataReceived;

        [ObservableProperty]
        private string _commandText;

        public TerminalPageViewModel(SshClientContainer sshClientContainer)
        {
            _sshClient = sshClientContainer.SshClient;
            _shellStream = _sshClient.CreateShellStream("xterm", 0, 0, 0, 0, 0);

            _shellStream.DataReceived += (s, e) =>
            {
                if (_shellStream.DataAvailable)
                {
                    var data = _shellStream.Read();
                    ConsoleDataReceived?.Invoke(data);
                }
            };

            _shellStream.ErrorOccurred += (s, e) =>
            {
                _ = DisplayAlert("Error", e.Exception.Message, "OK");
            };

            Task.Run(() =>
            {
                while (ConsoleDataReceived is null)
                {
                    // waiting
                }
                _shellStream.WriteLine("");
            });
        }

        [RelayCommand]
        private void Execute()
        {
            try
            {
                _shellStream.WriteLine(CommandText);
                CommandText = string.Empty;
            }
            catch { }
        }
    }
}
