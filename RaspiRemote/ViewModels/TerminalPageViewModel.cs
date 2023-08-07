using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Renci.SshNet;

namespace RaspiRemote.ViewModels
{
    internal partial class TerminalPageViewModel : BaseViewModel
    {
        private readonly SshClient _sshClient;
        private ShellStream _shellStream;

        public event Action<string> ConsoleDataReceived;

        [ObservableProperty]
        private string _commandText;

        public TerminalPageViewModel(SshClientContainer sshClientContainer)
        {
            _sshClient = sshClientContainer.SshClient;

            _ = ConfigureShellStream();
        }

        private async Task ConfigureShellStream() => await InvokeAsyncWithLoader(() =>
        {
            while (ConsoleDataReceived is null)
            {
                Thread.Sleep(100); // waiting for console view to initialize
            }

            try
            {
                _shellStream = _sshClient.CreateShellStream("xterm", 0, 0, 0, 0, 0);
                SetupShellStreamEvents();
            }
            catch (Exception ex)
            {
                _ = DisplayError(ex.Message);
            }
        });

        private void SetupShellStreamEvents()
        {
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
        }

        [RelayCommand]
        private void Send(string cmdText)
        {
            try
            {
                _shellStream.WriteLine(cmdText);
                CommandText = string.Empty;
            }
            catch (ObjectDisposedException)
            {
                _ = HandleShellInterrupted();
            }
            catch (Exception ex)
            {
                _ = DisplayError(ex.Message);
            }
        }

        private async Task HandleShellInterrupted()
        {
            var result = await DisplayAlert("Shell interrupted", "Do you want to reconnect to the shell?", "Yes", "No");
            if (result)
            {
                _ = ConfigureShellStream();
            }
        }
    }
}
