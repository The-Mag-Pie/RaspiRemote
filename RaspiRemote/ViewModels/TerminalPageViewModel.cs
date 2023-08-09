using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RaspiRemote.Parsers;
using Renci.SshNet;

namespace RaspiRemote.ViewModels
{
    internal partial class TerminalPageViewModel : BaseViewModel
    {
        private readonly SshClient _sshClient;
        private ShellStream _shellStream;

        public event Action<string> ConsoleDataReceived;
        public (double, double) ConsoleDimensions { private get; set; } = (0, 0);

        [ObservableProperty]
        private string _commandText;

        [ObservableProperty]
        private bool _sendCtrlKey;

        public TerminalPageViewModel(SshClientContainer sshClientContainer)
        {
            _sshClient = sshClientContainer.SshClient;

            _ = ConfigureShellStream();
        }

        private async Task ConfigureShellStream() => await InvokeAsyncWithLoader(() =>
        {
            while (ConsoleDataReceived is null || ConsoleDimensions == (0, 0))
            {
                Thread.Sleep(100); // waiting for console view to initialize
            }

            try
            {
                var colsAndRows = GetColsAndRows();
                _shellStream = _sshClient.CreateShellStream("xterm", colsAndRows.Item1, colsAndRows.Item2, 0, 0, 0);
                SetupShellStreamEvents();
            }
            catch (Exception ex)
            {
                _ = DisplayError(ex.Message);
            }
        });

        private (uint, uint) GetColsAndRows()
        {
            var cellWidth = 7;
            var cellHeight = 14;

#if WINDOWS
            // 15 is the width of scrollbar, 2 is a margin of error
            var consoleCols = (uint)((ConsoleDimensions.Item1 - 15) / cellWidth) - 2;
            var consoleRows = (uint)(ConsoleDimensions.Item2 / cellHeight);
#elif ANDROID
            // 5 is a margin of error
            var consoleCols = (uint)(ConsoleDimensions.Item1 / cellWidth) - 5;
            // 0.5 is a number representing how much of the screen the console takes while soft keyboard is open (more or less)
            var consoleRows = (uint)(ConsoleDimensions.Item2 * 0.4 / cellHeight);
#endif

            return (consoleCols, consoleRows);
        }

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
        private void HandleSendBtn()
        {
            try
            {
                if (SendCtrlKey)
                {
                    var ctrlKey = CtrlCharacterParser.GetCtrlCharacter(CommandText);
                    Send(ctrlKey.ToString());
                    SendCtrlKey = false;
                }
                else
                {
                    SendLine(CommandText);
                }
                CommandText = string.Empty;
            }
            catch (Exception ex)
            {
                _ = DisplayError(ex.Message);
            }
        }

        private void SendLine(string cmdText)
        {
            try
            {
                _shellStream.WriteLine(cmdText);
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

        private void Send(string cmdText)
        {
            try
            {
                _shellStream.Write(cmdText);
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

        [RelayCommand]
        private void Up() => Send("\x1B[A");

        [RelayCommand]
        private void Down() => Send("\x1B[B");

        [RelayCommand]
        private void Left() => Send("\x1B[D");

        [RelayCommand]
        private void Right() => Send("\x1B[C");

        [RelayCommand]
        private void Tab() => Send(((char)0x09).ToString());

        [RelayCommand]
        private void Esc() => Send(((char)0x1B).ToString());

        [RelayCommand]
        private void ChangeSendCtrlKey() => SendCtrlKey = !SendCtrlKey;
    }
}
